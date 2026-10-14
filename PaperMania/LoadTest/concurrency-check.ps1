# 재화 갱신 손실 검증: 같은 유저에게 골드 획득(PATCH /api/v3/player/currency/gold/gain)을 동시에 보내고
# 최종 골드가 (요청 수 + 워밍업 100건) x 지급량과 같은지 확인
# 사용: powershell -ExecutionPolicy Bypass -File LoadTest/concurrency-check.ps1 -ServerDir <Server.dll 이 있는 net9.0 폴더>
param([Parameter(Mandatory)][string]$ServerDir, [int]$Requests = 1000, [int]$Gold = 10, [string]$Label = "gold-concurrency")
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

docker compose up -d --wait
if ($LASTEXITCODE) { throw "docker compose 실패" }
$sql = {
    param($q)
    $q | docker exec -i loadtest-db-1 psql -U loadtest -d papermania -tA -q
    if ($LASTEXITCODE) { throw "psql 실패" }
}
& $sql "update paper_mania_game_data.player_currency_data set gold = 0 where user_id = 1" | Out-Null

dotnet build -c Release | Out-Null
if ($LASTEXITCODE) { throw "LoadTest 빌드 실패" }

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://127.0.0.1:80"
$env:Azure__UseKeyVault = "false"
$env:StageRewardCsvUrl = "http://127.0.0.1:5990/stage-reward.csv"
$env:LevelDefinitionCsvUrl = "http://127.0.0.1:5990/level.csv"
$env:CharacterDataCsvUrl = "http://127.0.0.1:5990/character.csv"
$env:Database__ConnectionStringSecretName = "LoadTestDb"
$env:LoadTestDb = "Host=127.0.0.1;Port=5441;Username=loadtest;Password=loadtest;Database=papermania"
$env:Redis__ConnectionString = "127.0.0.1:6380,abortConnect=false"
$env:Logging__LogLevel__Default = "Warning"
$env:LOADTEST_PLAYER_ID = "loadtest"
$env:LOADTEST_PASSWORD = "loadtest1234"
$env:LOADTEST_USERS = "1"   # 전부 같은 유저(같은 행)에 갱신을 몰아줌
$env:LOADTEST_METHOD = "PATCH"
$env:LOADTEST_PATH = "/api/v3/player/currency/gold/gain"
$env:LOADTEST_BODY = "{`"Gold`":$Gold}"
$env:LOADTEST_ITERATIONS = "$Requests"
$env:LOADTEST_LABEL = $Label

docker exec loadtest-redis-1 redis-cli flushall | Out-Null
$server = Start-Process dotnet "Server.dll" -WorkingDirectory $ServerDir -PassThru -NoNewWindow `
    -RedirectStandardOutput "server-$Label.log" -RedirectStandardError "server-$Label.err.log"
try {
    while ($true) {
        try { (New-Object Net.Sockets.TcpClient).Connect("127.0.0.1", 80); break }
        catch { if ($server.HasExited) { throw "서버 종료됨: LoadTest/server-$Label.err.log 확인" }; Start-Sleep 1 }
    }
    dotnet run --no-build -c Release | Out-Host
    if ($LASTEXITCODE) { throw "LoadTest 실행 실패" }
}
finally { Stop-Process $server -Force }

# 실패한 요청은 지급되지 않아야 하므로 성공 건수(+ 워밍업 100건) 기준으로 기대값 계산
$ok = [int]([regex]::Match((Get-Content "reports/player-data-$Label.md" -Raw), 'ok = `(\d+)`').Groups[1].Value)
$expected = ($ok + 100) * $Gold
$actual = [int](& $sql "select gold from paper_mania_game_data.player_currency_data where user_id = 1")
"$Label | 성공 $ok/$($Requests)건, 기대 골드 $expected, 실제 골드 $actual, 손실 $($expected - $actual) ($([math]::Round(($expected - $actual) / $Gold))건분)" |
    Tee-Object -FilePath "reports/$Label.txt"
