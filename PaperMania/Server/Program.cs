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
    ? "127.0.0.1:6379,abortConnect=false"
    : "redis:6379,abortConnect=false";
var redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddSingleton<StageRewardCache>();
builder.Services.AddSingleton<CharacterDataCache>();

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<SessionValidationFilter>();

const string keyname = "PaperManiaDbConnection";

builder.Services.AddScoped<IAccountRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyname];
    
    return new AccountRepository(connectionString!);
});
builder.Services.AddScoped<IDataRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyname];

    return new DataRepository(connectionString!);
});
builder.Services.AddScoped<ICurrencyRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyname];

    return new CurrencyRepository(connectionString!);
});
builder.Services.AddScoped<ICharacterRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyname];

    var cache = provider.GetRequiredService<CharacterDataCache>();
    
    return new CharacterRepository(connectionString!, cache);
});
builder.Services.AddScoped<IStageRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyname];
    
    return new StageRepository(connectionString!);
});
builder.Services.AddScoped<IRewardRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config[keyname];

    var cache = provider.GetRequiredService<StageRewardCache>();
    
    return new RewardRepository(connectionString!, cache);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v3",
        Title = "PaperMania API",
        Description = "API Version"
    });
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

using (var scope = app.Services.CreateScope())
{
    var stageRewardCache = scope.ServiceProvider.GetRequiredService<StageRewardCache>();
    await stageRewardCache.Initialize();
    
    var characterDataCache = scope.ServiceProvider.GetRequiredService<CharacterDataCache>();
    await characterDataCache.Initialize();
}

app.Run();