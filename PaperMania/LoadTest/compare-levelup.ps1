# 정적 데이터(레벨 정의) DB 조회 -> 메모리 조회 전환 비교: 경험치 획득 API(PATCH /api/v3/player/data/level)
# 옛 코드(레벨업마다 level_definition 조회)와 현재 코드를 같은 Docker/지연 환경에서 번갈아 측정
# 사용: powershell -ExecutionPolicy Bypass -File LoadTest/compare-levelup.ps1 -OldServerDir <옛 커밋 PaperMania/Server 경로>
param([Parameter(Mandatory)][string]$OldServerDir, [int]$DbLatencyMs = 20, [int[]]$Levels = @(1, 5, 10))
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

docker compose up -d --wait
if ($LASTEXITCODE) { throw "docker compose 실패" }
$psql = { param($sql) for ($i = 0; $i -lt 3; $i++) { $r = docker exec loadtest-db-1 psql -U loadtest -d papermania -tAc $sql; if ($LASTEXITCODE -eq 0 -and "$r".Trim() -ne "") { return $r }; Start-Sleep 1 } }  # docker exec가 가끔 빈 값을 반환해 재시도
Get-Content levelup.sql -Raw -Encoding UTF8 | docker exec -i loadtest-db-1 psql -U loadtest -d papermania -q
if ($LASTEXITCODE) { throw "시드 실패" }
& $psql "create extension if not exists pg_stat_statements" | Out-Null

dotnet build ../Server -c Release; if ($LASTEXITCODE) { throw "새 서버 빌드 실패" }
dotnet build $OldServerDir -c Release; if ($LASTEXITCODE) { throw "옛 서버 빌드 실패" }
dotnet build -c Release; if ($LASTEXITCODE) { throw "LoadTest 빌드 실패" }

$ua = "compare-levelup.ps1" # Toxiproxy는 브라우저(Mozilla) User-Agent 요청을 403으로 거부함
Invoke-RestMethod -Method Post http://127.0.0.1:8474/reset -UserAgent $ua | Out-Null
foreach ($t in @(@{ P = "postgres"; Ms = $DbLatencyMs; Jitter = 2 }, @{ P = "redis"; Ms = 1; Jitter = 0 })) {
    $body = @{ name = "latency"; type = "latency"; stream = "downstream"; toxicity = 1
               attributes = @{ latency = $t.Ms; jitter = $t.Jitter } } | ConvertTo-Json
    Invoke-RestMethod -Method Post "http://127.0.0.1:8474/proxies/$($t.P)/toxics" -UserAgent $ua -ContentType application/json -Body $body | Out-Null
}

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://127.0.0.1:80" # 옛 코드는 포트가 고정돼 있지 않음 (새 코드는 코드에서 80 고정)
$env:Azure__UseKeyVault = "false"
$env:StageRewardCsvUrl = "http://127.0.0.1:5990/stage-reward.csv"
$env:LevelDefinitionCsvUrl = "http://127.0.0.1:5990/level.csv"
$env:CharacterDataCsvUrl = "http://127.0.0.1:5990/character.csv"
$env:Database__ConnectionStringSecretName = "LoadTestDb"
$env:LoadTestDb = "Host=127.0.0.1;Port=15432;Username=loadtest;Password=loadtest;Database=papermania"
$env:Redis__ConnectionString = "127.0.0.1:16379,abortConnect=false"
$env:Logging__LogLevel__Default = "Warning"
$env:LOADTEST_PLAYER_ID = "loadtest"
$env:LOADTEST_PASSWORD = "loadtest1234"
$env:LOADTEST_USERS = "50" # 요청자마다 별도 계정 (같은 행에 대한 잠금 경쟁 제거)
$env:LOADTEST_METHOD = "PATCH"
$env:LOADTEST_PATH = "/api/v3/player/data/level"

$versions = @(@{ Name = "old"; Dir = (Resolve-Path "$OldServerDir/bin/Release/net9.0") },
              @{ Name = "new"; Dir = (Resolve-Path "../Server/bin/Release/net9.0") })
$result = foreach ($n in $Levels) {
    foreach ($v in $versions) {
        & $psql "update paper_mania_game_data.player_game_data set player_level = 1, player_exp = 0" | Out-Null
        & $psql "update paper_mania_game_data.player_currency_data set action_point = 100, action_point_max = 100" | Out-Null
        docker exec loadtest-redis-1 redis-cli flushall | Out-Null
        $label = "levelup-$($v.Name)-${n}lv"
        $server = Start-Process dotnet "Server.dll" -WorkingDirectory $v.Dir -PassThru -NoNewWindow `
            -RedirectStandardOutput "server-$label.log" -RedirectStandardError "server-$label.err.log"
        try {
            while ($true) {
                try { (New-Object Net.Sockets.TcpClient).Connect("127.0.0.1", 80); break }
                catch {
                    if ($server.HasExited) { throw "서버 종료됨: LoadTest/server-$label.err.log 확인" }
                    Start-Sleep 1
                }
            }
            $env:LOADTEST_LABEL = $label
            $env:LOADTEST_BODY = "{`"NewExp`":$($n * 100)}" # 레벨당 max_exp=100 이므로 정확히 n레벨 상승
            & $psql "select pg_stat_statements_reset()" | Out-Null
            dotnet run --no-build -c Release | Out-Host # 출력이 결과 객체에 섞이지 않게
            if ($LASTEXITCODE) { throw "LoadTest 실행 실패 ($label)" }
            $q = & $psql "select coalesce(sum(calls),0) from pg_stat_statements where query ~* 'player_game_data|level_definition|player_currency_data'"
            $lv = & $psql "select coalesce(sum(calls),0) from pg_stat_statements where query ~* 'level_definition'"
            [pscustomobject]@{ Label = $label; Queries = [int]$q; LevelDefQueries = [int]$lv }
        }
        finally { Stop-Process $server -Force }
    }
}
$result | Format-Table -AutoSize | Out-String | Tee-Object -FilePath "reports/levelup-queries.txt"
