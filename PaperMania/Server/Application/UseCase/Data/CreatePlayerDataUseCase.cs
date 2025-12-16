using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Data.Command;
using Server.Application.UseCase.Data.Result;

namespace Server.Application.UseCase.Data;

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
        var existName = await _dataRepository.ExistsPlayerNameAsync(request.PlayerName);
        if (existName != null)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_NAME_EXIST");

        var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId);

        var isNewAccount = await _accountRepository.IsNewAccountAsync(userId);
        if (!isNewAccount)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_DATA_EXIST");
    
        await _transactionScope.ExecuteAsync(async () =>
        {
            await _dataRepository.CreateDataAsync(userId, request.PlayerName);
            await _currencyRepository.CreateByUserIdAsync(userId);
            await _stageRepository.CreatePlayerStageDataAsync(userId);
            await _accountRepository.UpdateIsNewAccountAsync(userId, false);
        });

        return new AddPlayerDataResult(
            PlayerName: request.PlayerName
        );
    }
}