    using Server.Api.Dto.Response;
    using Server.Application.Exceptions;
    using Server.Application.Port;
    using Server.Application.UseCase.Data.Command;
    using Server.Application.UseCase.Data.Result;

    namespace Server.Application.UseCase.Data;

    public class GetPlayerNameByUserIdUseCase : IGetPlayerNameByUserIdUseCase
    {
        private readonly IDataRepository _repository;

        public GetPlayerNameByUserIdUseCase(
            IDataRepository repository
            )
        {
            _repository = repository;
        }
        
        public async Task<GetPlayerNameByUserIdResult> ExecuteAsync(GetPlayerNameByUserIdCommand request)
        {
            var data = await _repository.FindByUserIdAsync(request.UserId);
            if (data == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");

            return new GetPlayerNameByUserIdResult(
                PlayerName:data.PlayerName
            );
        }
    }