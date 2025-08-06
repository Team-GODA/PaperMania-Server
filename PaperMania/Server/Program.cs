using System.Text;
using Azure.Identity;
using Server.Api.Filter;
using Server.Api.Middleware;
using Server.Application.Configure;
using Server.Application.Port;
using Server.Infrastructure.Repository;
using Server.Infrastructure.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = "papermaniadbconnection";
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

var env = builder.Environment;

var redisConnectionString = env.IsDevelopment()
    ? "localhost:6379"
    : "redis:6379,abortConnect=false";
var redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.Configure<GoogleAuthSetting>(builder.Configuration.GetSection("GoogleAuth"));

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<SessionValidationFilter>();

var keyName = "DbConnectionString";

builder.Services.AddScoped<IAccountRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyName];

    return new AccountRepository(connectionString!);
});
builder.Services.AddScoped<IDataRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyName];

    return new DataRepository(connectionString!);
});
builder.Services.AddScoped<ICurrencyRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyName];

    return new CurrencyRepository(connectionString!);
});
builder.Services.AddScoped<ICharacterRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyName];

    return new CharacterRepository(connectionString!);
});
builder.Services.AddScoped<IStageRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyName];

    return new StageRepository(connectionString!);
});
builder.Services.AddScoped<IRewardRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyName];

    return new RewardRepository(connectionString!);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "PaperMania API",
        Description = "API Version 1"
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SessionRefresh>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();