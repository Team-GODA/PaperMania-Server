using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.In.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller.Currency
{
    [Route("api/v3/player/currency/paper-piece")]
    [ApiController]
    [SessionAuthorize]
    public class PaperPieceController : BaseController
    {
        private readonly IGetPaperPieceUseCase _getPaperPieceUseCase;
        private readonly ISpendPaperPieceUseCase _spendPaperPieceUseCase;
        private readonly ILogger<PaperPieceController> _logger;
        
        public PaperPieceController(
            IGetPaperPieceUseCase getPaperPieceUseCase,
            ISpendPaperPieceUseCase spendPaperPieceUseCase,
            ILogger<PaperPieceController> logger
            )
        {
            _getPaperPieceUseCase = getPaperPieceUseCase;
            _spendPaperPieceUseCase = spendPaperPieceUseCase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<GetPaperPieceResponse>> GetPaperPiece()
        {
            _logger.LogInformation(
                "[PaperPiece][GET] Request - UserId: {UserId}",
                UserId
            );
            
            var result = await _getPaperPieceUseCase.ExecuteAsync(new GetPaperPieceCommand(
                UserId)
            );

            var response = new GetPaperPieceResponse
            {
                PaperPiece = result.PaperPiece
            };
            
            _logger.LogInformation(
                "[PaperPiece][GET] Success - UserId: {UserId}, PaperPiece: {PaperPiece}",
                UserId, result.PaperPiece
            );

            return Ok(ApiResponse.Ok("플레이어 종이 조각 조회 성공", response));
        }

        [HttpPatch]
        public async Task<ActionResult<SpendPaperPieceResponse>> SpendPaperPiece(
            [FromBody] SpendPaperPieceRequest request)
        {
            _logger.LogInformation(
                "[PaperPiece][PATCH] Request - UserId: {UserId}",
                UserId
            );          

            var result = await _spendPaperPieceUseCase.ExecuteAsync(new SpendPaperPieceCommand(
                UserId, request.PaperPiece)
            );

            var response = new SpendPaperPieceResponse
            {
                PaperPiece = result.PaperPiece
            };
            
            _logger.LogInformation(
                "[PaperPiece][PATCH] Success - UserId: {UserId}, PaperPiece: {PaperPiece}",
                UserId, result.PaperPiece
            );

            return Ok(ApiResponse.Ok("플레이어 종이 조각 사용 성공", response));
        }
    }
}
