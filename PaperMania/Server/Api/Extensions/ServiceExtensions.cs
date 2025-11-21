using Server.Api.Filter;
using Server.Application.Port;
using Server.Infrastructure.Repository;
using Server.Infrastructure.Service;
using StackExchange.Redis;

namespace Server.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository>(provider =>
        {
            var connectionString = GetConnectionString(provider);
            return new AccountRepository(connectionString);
        });
        
        services.AddScoped<IDataRepository>(provider =>
        {
            var connectionString = GetConnectionString(provider);
            return new DataRepository(connectionString);
        });
        
        services.AddScoped<ICurrencyRepository>(provider =>
        {
            var connectionString = GetConnectionString(provider);
            return new CurrencyRepository(connectionString);
        });
        
        services.AddScoped<ICharacterRepository>(provider =>
        {
            var connectionString = GetConnectionString(provider);
            var cache = provider.GetRequiredService<CharacterDataCache>();
            return new CharacterRepository(connectionString, cache);
        });
        
        services.AddScoped<IStageRepository>(provider =>
        {
            var connectionString = GetConnectionString(provider);
            return new StageRepository(connectionString);
        });
        
        services.AddScoped<IRewardRepository>(provider =>
        {
            var connectionString = GetConnectionString(provider);
            var cache = provider.GetRequiredService<StageRewardCache>();
            return new RewardRepository(connectionString, cache);
        });
        
        return services;
    }

    private static string GetConnectionString(IServiceProvider provider)
    {
        var config = provider.GetRequiredService<IConfiguration>();
        var keyName = config["DataBase:ConnectionStringKey"];
        
        var connectionString = config[keyName];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"DB 연결 문자열을 찾을 수 없습니다. Key: {keyName}");
        }
        
        return connectionString;
    }

    public static IServiceCollection AddCache(
        this IServiceCollection services,
        string redisConnectionString)
    {
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddSingleton<StageRewardCache>();
        services.AddSingleton<CharacterDataCache>();
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
    
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IRewardService, RewardService>();
        services.AddScoped<SessionValidationFilter>();
        
        return services;
    }
}