using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Cache;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Player;

    public class GetPlayerNameUseCase : IGetPlayerNameUseCase
    {
        private readonly IDataRepository _repository;
        private readonly ICacheAsideService _cache;

        public GetPlayerNameUseCase(
            IDataRepository repository,
            ICacheAsideService cache
            )
        {
            _repository = repository;
            _cache = cache;
        }
        
        public async Task<GetPlayerNameResult> ExecuteAsync(GetPlayerNameCommand request, CancellationToken ct)
        {
            var player = await _cache.GetOrSetAsync(
                CacheKey.Profile.ByUserId(request.UserId),
                async (token) => await _repository.FindByUserIdAsync(request.UserId, token),
                TimeSpan.FromDays(30),
                ct
            );
                
            if (player == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");

            return new GetPlayerNameResult(player.Name);
        }
    }