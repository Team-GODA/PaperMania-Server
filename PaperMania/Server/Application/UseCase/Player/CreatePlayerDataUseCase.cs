using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.In.Player;
using Server.Application.Port.Out.Infrastructure;
using Server.Application.Port.Out.Persistence;
using Server.Application.Port.Out.Service;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Domain.Entity;

namespace Server.Application.UseCase.Player;

public class CreatePlayerDataUseCase : ICreatePlayerDataUseCase
{
    private readonly IDataRepository _dataRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ISessionService _sessionService;
    private readonly IStageRepository _stageRepository;
    private readonly ITransactionScope _transactionScope;

    public CreatePlayerDataUseCase(
        IDataRepository  dataRepository,
        IAccountRepository  accountRepository,
        ICurrencyRepository currencyRepository,
        ISessionService sessionService,
        IStageRepository stageRepository,
        ITransactionScope transactionScope)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _sessionService = sessionService;
        _stageRepository = stageRepository;
        _transactionScope = transactionScope;
    }
    
    public async Task<AddPlayerDataResult> ExecuteAsync(AddPlayerDataCommand request)
    {
        var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId);

        var account = await _accountRepository.FindByUserIdAsync(userId);
        if (account == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "ACCOUNT_NOT_FOUND");
        
        if (!account.IsNewAccount)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_DATA_EXIST");
    
        await _transactionScope.ExecuteAsync(async () =>
        {
            var player = new PlayerGameData
            {
                UserId = userId,
                PlayerName = request.PlayerName,
                PlayerLevel = 1,
                PlayerExp = 0
            };
            
            await _dataRepository.CreateAsync(player);
            await _currencyRepository.CreateByUserIdAsync(userId);
            await _stageRepository.CreatePlayerStageDataAsync(userId);
            
            account.IsNewAccount = false;
            await _accountRepository.UpdateAsync(account);
        });

        return new AddPlayerDataResult(
            PlayerName: request.PlayerName
        );
    }
}