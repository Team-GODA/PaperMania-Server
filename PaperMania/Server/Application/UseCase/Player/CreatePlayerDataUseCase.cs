using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Player;

public class CreatePlayerDataUseCase : ICreatePlayerDataUseCase
{
    private readonly IDataDao _dataDao;
    private readonly IAccountDao _accountDao;
    private readonly ICurrencyDao _currencyDao;
    private readonly ISessionService _sessionService;
    private readonly ITransactionScope _transactionScope;

    public CreatePlayerDataUseCase(
        IDataDao  dataDao,
        IAccountDao  accountDao,
        ICurrencyDao currencyDao,
        ISessionService sessionService,
        ITransactionScope transactionScope)
    {
        _dataDao = dataDao;
        _accountDao = accountDao;
        _currencyDao = currencyDao;
        _sessionService = sessionService;
        _transactionScope = transactionScope;
    }
    
    public async Task<AddPlayerDataResult> ExecuteAsync(AddPlayerDataCommand request)
    {
        var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId);

        var account = await _accountDao.FindByUserIdAsync(userId);
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
                Name = request.PlayerName,
                Level = 1,
                Exp = 0
            };
            
            await _dataDao.CreateAsync(player);
            await _currencyDao.CreateByUserIdAsync(userId);
            
            account.IsNewAccount = false;
            await _accountDao.UpdateAsync(account);
        });

        return new AddPlayerDataResult(
            PlayerName: request.PlayerName
        );
    }
}