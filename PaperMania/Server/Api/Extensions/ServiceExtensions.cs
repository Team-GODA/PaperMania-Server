using Server.Api.Filter;
using Server.Application.Port.Input.Auth;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
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
        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRegisterUseCase ,RegisterUseCase>();
        services.AddScoped<ILogoutUseCase ,LogoutUseCase>();
        services.AddScoped<IValidateUseCase ,ValidateUseCase>();
        
        // player use case
        services.AddScoped<ICreatePlayerDataUseCase, CreatePlayerDataUseCase>();
        services.AddScoped<GetPlayerNameUseCase ,GetPlayerNameUseCase>();
        services.AddScoped<IGetPlayerLevelUseCase, GetPlayerLevelUseCase>();
        services.AddScoped<IGainPlayerExpUseCase, GainPlayerExpUseCase>();
        services.AddScoped<IRenameUseCase, RenameUseCase>();
        
        // currency use case
        services.AddScoped<IGetActionPointUseCase, GetActionPointUseCase>();
        services.AddScoped<IUpdateMaxActionPointUseCase, UpdateMaxActionPointUseCase>();
        services.AddScoped<ISpendActionPointUseCase, SpendActionPointUseCase>();
        services.AddScoped<IGainGoldUseCase, GainGoldUseCase>();
        services.AddScoped<IGetGoldUseCase, GetGoldUseCase>();
        services.AddScoped<ISpendGoldUseCase, SpendGoldUseCase>();
        services.AddScoped<IGainPaperPieceUseCase, GainPaperPieceUseCase>();
        services.AddScoped<IGetPaperPieceUseCase, GetPaperPieceUseCase>();
        services.AddScoped<ISpendPaperPieceUseCase, SpendPaperPieceUseCase>();
        
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ActionPointService>();
        
        services.AddScoped<CacheWrapper>();
        
        return services;
    }
}