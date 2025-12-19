using Server.Api.Filter;
using Server.Application.Port.Out.Infrastructure;
using Server.Application.Port.Out.Persistence;
using Server.Application.Port.Out.Service;
using Server.Application.UseCase.Auth;
using Server.Application.UseCase.Currency;
using Server.Application.UseCase.Player;
using Server.Domain.Service;
using Server.Infrastructure.Cache;
using Server.Infrastructure.Repository;
using Server.Infrastructure.Service;
using StackExchange.Redis;

namespace Server.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<ITransactionScope>(provider =>
        {
            var connectionString = GetConnectionString(provider);
            return new TransactionScope(connectionString);
        });
        
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
    
    public static IServiceCollection AddApiFilters(
        this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ApiLogActionFilter>();
        });

        return services;
    }

    private static string GetConnectionString(IServiceProvider provider)
    {
        var config = provider.GetRequiredService<IConfiguration>();
        var keyName = config["Database:ConnectionStringSecretName"];

        if (string.IsNullOrEmpty(keyName))
            throw new InvalidOperationException(
                $"DB 연결 KeyName을 찾을 수 없습니다. KeyName: {keyName}");
            
        var connectionString = config[keyName];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException(
                $"DB 연결 문자열을 찾을 수 없습니다. Key: {keyName}");
        
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
        services.AddScoped<SessionValidationFilter>();

        // auth use case
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<ValidateUseCase>();
        
        // player use case
        services.AddScoped<CreatePlayerDataUseCase>();
        services.AddScoped<GetPlayerNameUseCase>();
        services.AddScoped<GetPlayerLevelUseCase>();
        services.AddScoped<GainPlayerExpUseCase>();
        services.AddScoped<RenameUseCase>();
        
        // currency use case
        services.AddScoped<GetActionPointUseCase>();
        services.AddScoped<UpdateMaxActionPointUseCase>();
        services.AddScoped<SpendActionPointUseCase>();
        services.AddScoped<GainGoldUseCase>();
        services.AddScoped<GetGoldUseCase>();
        services.AddScoped<SpendGoldUseCase>();
        services.AddScoped<GainPaperPieceUseCase>();
        services.AddScoped<GetPaperPieceUseCase>();
        services.AddScoped<SpendPaperPieceUseCase>();
        
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ActionPointService>();
        
        services.AddScoped<CacheWrapper>();
        
        return services;
    }
}