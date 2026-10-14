using System.Net.Http.Json;
using System.Text;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;

// 플레이어 데이터 조회 API(GET /api/v3/player/data/level) Redis 캐싱 전후 비교
// 1) 서버 실행: as-is = Cache__Enabled=false, to-be = Cache__Enabled=true(기본값)
// 2) LOADTEST_LABEL=as-is|to-be, LOADTEST_PLAYER_ID, LOADTEST_PASSWORD 설정 후 dotnet run -c Release
// LOADTEST_PATH/METHOD/BODY: 다른 API 측정, LOADTEST_USERS>1: 동시 사용자마다 별도 계정(PLAYER_ID + 1..N) 사용
var baseUrl = Env("LOADTEST_BASE_URL", "http://127.0.0.1"); // localhost는 ::1 시도 후 폴백하며 연결이 지연됨
var label = Env("LOADTEST_LABEL", "run");
var copies = int.Parse(Env("LOADTEST_COPIES", "50"));
var iterations = int.Parse(Env("LOADTEST_ITERATIONS", "5000"));
var users = int.Parse(Env("LOADTEST_USERS", "1"));
var method = Env("LOADTEST_METHOD", "GET");
var body = Environment.GetEnvironmentVariable("LOADTEST_BODY");

using var httpClient = Http.CreateDefaultClient();

var sessions = new string[users];
for (var i = 0; i < users; i++)
{
    var playerId = users == 1 ? Env("LOADTEST_PLAYER_ID") : $"{Env("LOADTEST_PLAYER_ID")}{i + 1}";
    var login = await httpClient.PostAsJsonAsync($"{baseUrl}/api/v3/auth/login",
        new { PlayerId = playerId, Password = Env("LOADTEST_PASSWORD") });
    login.EnsureSuccessStatusCode();
    sessions[i] = (await login.Content.ReadFromJsonAsync<LoginBody>())!.Data.SessionId;
}

HttpRequestMessage Request(string sessionId)
{
    var request = Http.CreateRequest(method, $"{baseUrl}{Env("LOADTEST_PATH", "/api/v3/player/data/level")}")
        .WithHeader("Session-Id", sessionId);
    return body is null ? request : request.WithBody(new StringContent(body, Encoding.UTF8, "application/json"));
}

// JIT/커넥션 풀 워밍업 (to-be는 여기서 캐시도 채워짐)
for (var i = 0; i < 100; i++)
    (await httpClient.SendAsync(Request(sessions[i % users]))).EnsureSuccessStatusCode();

var scenario = Scenario.Create("get_player_data",
        async context => await Http.Send(httpClient, Request(sessions[context.ScenarioInfo.InstanceNumber % users])))
    .WithoutWarmUp()
    .WithLoadSimulations(Simulation.IterationsForConstant(copies, iterations));

NBomberRunner
    .RegisterScenarios(scenario)
    .WithTestName(label)
    .WithReportFolder("reports")
    .WithReportFileName($"player-data-{label}")
    .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
    .Run();

static string Env(string key, string? fallback = null) =>
    Environment.GetEnvironmentVariable(key) ?? fallback
    ?? throw new InvalidOperationException($"환경변수 {key}가 필요합니다.");

record LoginBody(LoginData Data);
record LoginData(string SessionId);
