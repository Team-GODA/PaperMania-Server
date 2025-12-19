using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetCurrencyDataUseCase : IGetCurrencyDataUseCase
{
    private readonly ICurrencyRepository _repository;

    public GetCurrencyDataUseCase(ICurrencyRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<GetCurrencyDataResult> ExecuteAsync(GetCurrencyDataCommand request)
    {
        request.Validate();
        
        var data = await _repository.FindByUserIdAsync(request.UserId)
            ?? throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND"
                );

        return new GetCurrencyDataResult(
            data.ActionPoint,
            data.Gold,
            data.PaperPiece
            );
    }
}