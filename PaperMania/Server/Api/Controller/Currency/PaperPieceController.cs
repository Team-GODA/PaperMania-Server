using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request.Currency;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.Input.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller.Currency
{
    [Route("api/v3/player/currency/paper-piece")]
    [ApiController]
    [SessionAuthorize]
    public class PaperPieceController : BaseController
    {
        private readonly IGetPaperPieceUseCase _getPaperPieceUseCase;
        private readonly IGainPaperPieceUseCase _gainPaperPieceUseCase;
        private readonly ISpendPaperPieceUseCase _spendPaperPieceUseCase;
        
        public PaperPieceController(
            IGetPaperPieceUseCase getPaperPieceUseCase,
            IGainPaperPieceUseCase gainPaperPieceUseCase,
            ISpendPaperPieceUseCase spendPaperPieceUseCase
            )
        {
            _getPaperPieceUseCase = getPaperPieceUseCase;
            _gainPaperPieceUseCase = gainPaperPieceUseCase;
            _spendPaperPieceUseCase = spendPaperPieceUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse<GetPaperPieceResponse>>> GetPaperPiece(CancellationToken ct)
        {
            var userId = GetUserId();
            
            var result = await _getPaperPieceUseCase.ExecuteAsync(new GetPaperPieceCommand(
                userId),
                ct);

            var response = new GetPaperPieceResponse
            {
                PaperPiece = result.PaperPiece
            };
            
            return Ok(ApiResponse.Ok("플레이어 종이 조각 조회 성공", response));
        }

        [HttpPatch("gain")]
        public async Task<ActionResult<BaseResponse<GainPaperPieceResponse>>> GainPaperPiece(
            [FromBody] GainPaperPieceRequest request,
            CancellationToken ct)
        {
            var userId = GetUserId();
            
            var result = await _gainPaperPieceUseCase.ExecuteWithTransactionAsync(new GainPaperPieceCommand(
                userId, request.PaperPiece),
                ct);
            
            var response = new GainPaperPieceResponse
            {
                PaperPiece = result.PaperPiece
            };

            return Ok(ApiResponse.Ok("플레이어 종이 조각 증가 성공", response));
        }

        [HttpPatch("spend")]
        public async Task<ActionResult<BaseResponse<SpendPaperPieceResponse>>> SpendPaperPiece(
            [FromBody] SpendPaperPieceRequest request,
            CancellationToken ct)
        {
            var userId = GetUserId();
            
            var result = await _spendPaperPieceUseCase.ExecuteAsync(new SpendPaperPieceCommand(
                userId, request.PaperPiece),
                ct);

            var response = new SpendPaperPieceResponse
            {
                PaperPiece = result.PaperPiece
            };
            
            return Ok(ApiResponse.Ok("플레이어 종이 조각 사용 성공", response));
        }
    }
}