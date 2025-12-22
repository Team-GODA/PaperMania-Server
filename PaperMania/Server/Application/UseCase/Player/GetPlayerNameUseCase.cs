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
        private readonly IDataDao _dao;
        private readonly CacheWrapper _cache;

        public GetPlayerNameUseCase(
            IDataDao dao,
            CacheWrapper cache
            )
        {
            _dao = dao;
            _cache = cache;
        }
        
        public async Task<GetPlayerNameResult> ExecuteAsync(GetPlayerNameCommand request)
        {
            var playerName = await _cache.FetchAsync(
                CacheKey.Profile.ByUserId(request.UserId),
                async () =>
                {
                    var data = await _dao.FindByUserIdAsync(request.UserId);
                    return data?.PlayerName;
                },
                TimeSpan.FromDays(30)
            );
                
            if (playerName == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");

            return new GetPlayerNameResult(
                PlayerName: playerName
            );
        }
    }