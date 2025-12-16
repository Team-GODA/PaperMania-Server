    using Server.Api.Dto.Response;
    using Server.Application.Exceptions;
    using Server.Application.Port;
    using Server.Application.UseCase.Data.Command;
    using Server.Application.UseCase.Data.Result;
    using Server.Infrastructure.Cache;

    namespace Server.Application.UseCase.Data;

    public class GetPlayerNameByUserIdUseCase : IGetPlayerNameByUserIdUseCase
    {
        private readonly IDataRepository _repository;
        private readonly CacheWrapper _cache;

        public GetPlayerNameByUserIdUseCase(
            IDataRepository repository,
            CacheWrapper cache
            )
        {
            _repository = repository;
            _cache = cache;
        }
        
        public async Task<GetPlayerNameByUserIdResult> ExecuteAsync(GetPlayerNameByUserIdCommand request)
        {
            var playerName = await _cache.FetchAsync<string>(
                CacheKey.Profile.ByUserId(request.UserId),
                async () =>
                {
                    var data = await _repository.FindByUserIdAsync(request.UserId);
                    return data?.PlayerName;
                },
                TimeSpan.FromDays(30)
            );
                
            if (playerName == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");

            return new GetPlayerNameByUserIdResult(
                PlayerName: playerName
            );
        }
    }