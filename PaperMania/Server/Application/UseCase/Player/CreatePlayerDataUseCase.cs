using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.Port.Output.Transaction;
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
    private readonly ITransactionScope _transactionScope;

    public CreatePlayerDataUseCase(
        IDataRepository  dataRepository,
        IAccountRepository  accountRepository,
        ICurrencyRepository currencyRepository,
        ISessionService sessionService,
        ITransactionScope transactionScope)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _sessionService = sessionService;
        _transactionScope = transactionScope;
    }
    
    public async Task<AddPlayerDataResult> ExecuteAsync(AddPlayerDataCommand request, CancellationToken ct)
    {
        var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId, ct);

        var account = await _accountRepository.FindByUserIdAsync(userId, ct);
        if (account == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "ACCOUNT_NOT_FOUND");
        
        if (!account.IsNewAccount)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_DATA_EXIST");
    
        await _transactionScope.ExecuteAsync(async (innerCt) =>
        {
            var player = new GameData(userId, request.PlayerName, 0, 1);
            
            await _dataRepository.CreateAsync(player, innerCt);
            await _currencyRepository.CreateByUserIdAsync(userId, innerCt);
            
            account.IsNewAccount = false;
            await _accountRepository.UpdateAsync(account, innerCt);
        }, ct);

        return new AddPlayerDataResult(
            PlayerName: request.PlayerName
        );
    }
}