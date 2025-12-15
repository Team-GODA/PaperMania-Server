using Server.Application.Port;
using Server.Application.UseCase.Currency.Command;

namespace Server.Application.UseCase.Currency;

public class RegenerateActionPointService : IRegenerateActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegenerateActionPointService(
        ICurrencyRepository repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ExecuteAsync(RegenerateActionPointCommand request)
    {
        var data = await _repository.FindPlayerCurrencyDataByUserIdAsync(request.UserId);
        
        await _unitOfWork.ExecuteAsync(async () =>
        {
            
        });
    }
}