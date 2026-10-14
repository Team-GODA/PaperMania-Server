# 로컬 Docker(Postgres + Redis)에서 as-is(캐시 off) -> to-be(캐시 on) 순서로 측정
# 사용: powershell -ExecutionPolicy Bypass -File LoadTest/run.ps1 [-DbLatencyMs 20 [-RedisLatencyMs 1]]
#   -DbLatencyMs > 0: Toxiproxy로 Postgres(왕복 지연)/Redis 앞에 지연을 넣어 원격 DB 환경을 흉내 냄
#   (prod의 Redis는 서버와 같은 Docker 네트워크라 기본값 1ms, DB는 별도 지정)
#   -Iterations: 총 요청 수(기본 5000). 기본값이 아니면 리포트 이름에 붙임
#     NBomber는 5초 간격으로만 시계열을 수집하므로 HTML 차트를 보려면 수십 초 이상 돌도록 늘릴 것 (예: 150000)
#   -Path: 측정할 API 경로 (기본 /api/v3/player/data/level). 기본값이 아니면 리포트 이름에 붙임
param([int]$DbLatencyMs = 0, [int]$RedisLatencyMs = 1, [int]$Iterations = 5000, [string]$Path = "/api/v3/player/data/level")
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

docker compose up -d --wait
if ($LASTEXITCODE) { throw "docker compose 실패 (Docker Desktop 실행 여부 확인)" }

$dbPort = 5441; $redisPort = 6380; $suffix = ""
$env:LOADTEST_ITERATIONS = "$Iterations"
$ua = "run.ps1" # Toxiproxy는 브라우저(Mozilla) User-Agent 요청을 403으로 거부함
if ($DbLatencyMs -gt 0) {
    $dbPort = 15432; $redisPort = 16379; $suffix = "-db${DbLatencyMs}ms"
    for ($i = 0; ; $i++) {
        try { Invoke-RestMethod -Method Post http://127.0.0.1:8474/reset -UserAgent $ua | Out-Null; break }  # 이전 지연 설정 초기화
        catch { if ($i -gt 20) { throw "Toxiproxy API 연결 실패" }; Start-Sleep 1 }
    }
    foreach ($t in @(@{ P = "postgres"; Ms = $DbLatencyMs; Jitter = 2 }, @{ P = "redis"; Ms = $RedisLatencyMs; Jitter = 0 })) {
        $body = @{ name = "latency"; type = "latency"; stream = "downstream"; toxicity = 1
                   attributes = @{ latency = $t.Ms; jitter = $t.Jitter } } | ConvertTo-Json
        Invoke-RestMethod -Method Post "http://127.0.0.1:8474/proxies/$($t.P)/toxics" -UserAgent $ua -ContentType application/json -Body $body | Out-Null
    }
}
if ($Iterations -ne 5000) { $suffix += "-${Iterations}req" }
$env:LOADTEST_PATH = $Path
if ($Path -ne "/api/v3/player/data/level") { $suffix += "-" + ($Path.Trim("/") -replace "[^A-Za-z0-9]+", "-") }
dotnet build ../Server -c Release
if ($LASTEXITCODE) { throw "서버 빌드 실패" }
dotnet build -c Release
if ($LASTEXITCODE) { throw "LoadTest 빌드 실패" }

# Key Vault 없이 로컬 DB/Redis/정적 데이터(nginx)로 연결
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:Azure__UseKeyVault = "false"
$env:StageRewardCsvUrl = "http://127.0.0.1:5990/stage-reward.csv"
$env:LevelDefinitionCsvUrl = "http://127.0.0.1:5990/level.csv"
$env:CharacterDataCsvUrl = "http://127.0.0.1:5990/character.csv"
$env:Database__ConnectionStringSecretName = "LoadTestDb"
$env:LoadTestDb = "Host=127.0.0.1;Port=$dbPort;Username=loadtest;Password=loadtest;Database=papermania"
$env:Redis__ConnectionString = "127.0.0.1:$redisPort,abortConnect=false"
$env:Logging__LogLevel__Default = "Warning" # 요청마다 찍히는 로그가 측정에 섞이지 않도록
$env:LOADTEST_PLAYER_ID = "loadtest"        # init.sql 시드 계정
$env:LOADTEST_PASSWORD = "loadtest1234"

$serverDir = Resolve-Path ../Server/bin/Release/net9.0

foreach ($run in @(@{ Label = "as-is"; Cache = "false" }, @{ Label = "to-be"; Cache = "true" })) {
    $env:Cache__Enabled = $run.Cache
    $server = Start-Process dotnet "Server.dll" -WorkingDirectory $serverDir -PassThru -NoNewWindow `
        -RedirectStandardOutput "server-$($run.Label).log" -RedirectStandardError "server-$($run.Label).err.log"
    try {
        # 정적 데이터(CSV) 로딩이 끝나야 80 포트가 열림
        while ($true) {
            try { (New-Object Net.Sockets.TcpClient).Connect("127.0.0.1", 80); break }
            catch {
                if ($server.HasExited) { throw "서버 종료됨: LoadTest/server-$($run.Label).log 확인" }
                Start-Sleep 1
            }
        }
        $env:LOADTEST_LABEL = "$($run.Label)$suffix"
        docker exec loadtest-redis-1 redis-cli flushall | Out-Null  # 이전 실행의 캐시(TTL 30일)가 남지 않게 콜드 스타트로 시작
        $psql = "docker", "exec", "loadtest-db-1", "psql", "-U", "loadtest", "-d", "papermania", "-tAc"
        & $psql[0] $psql[1..($psql.Length - 1)] "create extension if not exists pg_stat_statements" | Out-Null  # 기존 DB 볼륨에선 init.sql이 재실행되지 않음
        & $psql[0] $psql[1..($psql.Length - 1)] "select pg_stat_statements_reset()" | Out-Null
        dotnet run --no-build -c Release
        if ($LASTEXITCODE) { throw "LoadTest 실행 실패 ($($run.Label))" }
        # player_game_data 조회 쿼리 실행 횟수 (워밍업 100건 포함)
        $q = & $psql[0] $psql[1..($psql.Length - 1)] "select coalesce(sum(calls),0) from pg_stat_statements where query ~* '^\s*select.*from paper_mania_game_data.player_game_data'"
        "DB queries ($($run.Label)$suffix): $q" | Tee-Object -FilePath "reports/db-queries-$($run.Label)$suffix.txt"
        $all = & $psql[0] $psql[1..($psql.Length - 1)] "select coalesce(sum(calls),0) from pg_stat_statements where query ~* 'paper_mania_'"
        "DB queries, 앱 테이블 전체 ($($run.Label)$suffix): $all" | Tee-Object -Append -FilePath "reports/db-queries-$($run.Label)$suffix.txt"
    }
    finally { Stop-Process $server -Force }
}
