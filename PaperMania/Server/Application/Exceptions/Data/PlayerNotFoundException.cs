using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Data;

public class PlayerNotFoundException : GameException
{
    public PlayerNotFoundException(int? userId)
        : base(ErrorStatusCode.NotFound,
            userId.HasValue 
                ? $"{userId.Value} 에 대한 플레이어 데이터를 찾을 수 없습니다."
                : "플레이어 데이터를 찾을 수 없습니다.")
    {
    }
}