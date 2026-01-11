using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Player;

    public class GetPlayerNameUseCase : IGetPlayerNameUseCase
    {
        private readonly IDataRepository _repository;
        private readonly CacheAsideService _cache;

        public GetPlayerNameUseCase(
            IDataRepository repository,
            CacheAsideService cache
            )
        {
            _repository = repository;
            _cache = cache;
        }
        
        public async Task<GetPlayerNameResult> ExecuteAsync(GetPlayerNameCommand request)
        {
            var player = await _cache.GetOrSetAsync(
                CacheKey.Profile.ByUserId(request.UserId),
                async () => await _repository.FindByUserIdAsync(request.UserId),
                TimeSpan.FromDays(30)
            );
                
            if (player == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");

            return new GetPlayerNameResult(player.Name);
        }
    }