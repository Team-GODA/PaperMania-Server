# 트랜잭션 롤백 검증: 스테이지 보상 수령(POST /api/v3/reward/stage/1/1)의 마지막 단계(경험치 갱신)를 DB 트리거로 실패시킨 뒤
# 앞 단계(클리어 기록 / 골드 / 종이조각)가 함께 롤백되는지 확인
# 사용: powershell -ExecutionPolicy Bypass -File LoadTest/tx-check.ps1 -ServerDir <Server.dll 이 있는 net9.0 폴더>
param([Parameter(Mandatory)][string]$ServerDir, [switch]$NoInjection)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

docker compose up -d --wait
if ($LASTEXITCODE) { throw "docker compose 실패" }
$sql = {
    param($q)
    $q | docker exec -i loadtest-db-1 psql -U loadtest -d papermania -tA -q
    if ($LASTEXITCODE) { throw "psql 실패" }
}

& $sql @"
create table if not exists paper_mania_game_data.player_stage_data (
    user_id int not null, stage_num int not null, stage_sub_num int not null,
    primary key (user_id, stage_num, stage_sub_num));
delete from paper_mania_game_data.player_stage_data;
update paper_mania_game_data.player_currency_data set gold = 0, paper_piece = 0 where user_id = 1;
update paper_mania_game_data.player_game_data set player_level = 1, player_exp = 0 where user_id = 1;
create or replace function fail_exp_update() returns trigger as `$`$ begin raise exception 'injected failure'; end `$`$ language plpgsql;
drop trigger if exists fail_exp on paper_mania_game_data.player_game_data;
create trigger fail_exp before update on paper_mania_game_data.player_game_data
    for each row execute function fail_exp_update();
"@ | Out-Null
if ($NoInjection) { & $sql "drop trigger fail_exp on paper_mania_game_data.player_game_data" | Out-Null }

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

docker exec loadtest-redis-1 redis-cli flushall | Out-Null
$server = Start-Process dotnet "Server.dll" -WorkingDirectory $ServerDir -PassThru -NoNewWindow `
    -RedirectStandardOutput "server-txcheck.log" -RedirectStandardError "server-txcheck.err.log"
try {
    while ($true) {
        try { (New-Object Net.Sockets.TcpClient).Connect("127.0.0.1", 80); break }
        catch { if ($server.HasExited) { throw "서버 종료됨: LoadTest/server-txcheck.err.log 확인" }; Start-Sleep 1 }
    }
    $login = Invoke-RestMethod -Method Post http://127.0.0.1/api/v3/auth/login -ContentType application/json `
        -Body (@{ PlayerId = "loadtest"; Password = "loadtest1234" } | ConvertTo-Json)
    try {
        Invoke-RestMethod -Method Post http://127.0.0.1/api/v3/reward/stage/1/1 -Headers @{ "Session-Id" = $login.Data.SessionId } | Out-Null
        $status = 200
    }
    catch { $status = [int]$_.Exception.Response.StatusCode }
}
finally { Stop-Process $server -Force }

$gold = & $sql "select gold from paper_mania_game_data.player_currency_data where user_id = 1"
$piece = & $sql "select paper_piece from paper_mania_game_data.player_currency_data where user_id = 1"
$stage = & $sql "select count(*) from paper_mania_game_data.player_stage_data where user_id = 1"
& $sql "drop trigger if exists fail_exp on paper_mania_game_data.player_game_data" | Out-Null
"HTTP $status | gold=$gold paper_piece=$piece stage_rows=$stage  (실패 주입 시: 롤백 0 / 0 / 0, 미롤백 100 / 10 / 1. 주입 없으면 100 / 10 / 1)"
