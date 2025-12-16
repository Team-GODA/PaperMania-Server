using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Auth;
using Server.Application.UseCase.Auth;
using Server.Application.UseCase.Auth.Command;  

namespace Server.Api.Controller
{
    [Route("api/v3/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginUseCase _loginUseCase;
        private readonly RegisterUseCase _registerUseCase;
        private readonly LogoutUseCase _logoutUseCase;
        private readonly ValidateUseCase _validateUseCase;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            LoginUseCase loginUseCase,
            RegisterUseCase registerUseCase,
            LogoutUseCase logoutUseCase,
            ValidateUseCase validateUseCase,
            ILogger<AuthController> logger)
        {
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
            _logoutUseCase = logoutUseCase;
            _validateUseCase = validateUseCase;
            _logger = logger;
        }

        /// <summary>
        /// 세션 유효 인증 API
        /// </summary>
        /// <param name="request">유저 확인을 유한 header의 Session-Id의 값</param>
        /// <returns>유효한 유저 확인 후 해당 유저 ID와 성공 </returns>>
        [HttpPost("validate")]
        [SessionAuthorize]
        [ProducesResponseType(typeof(BaseResponse<ValidateUserResponse>), 200)]
        public async Task<ActionResult<BaseResponse<ValidateUserResponse>>> ValidateUser()
        {
            _logger.LogInformation("유저 인증 시도");
            
            var sessionId = HttpContext.Items["SessionId"] as string;

            await _validateUseCase.ExecuteAsync(sessionId);
            
            _logger.LogInformation($"유저 인증 성공");
        
            return Ok(ApiResponse.Ok<EmptyResponse>("유저 인증 성공"));
        }
        
        /// <summary>
        /// 신규 회원가입 API
        /// </summary>
        /// <param name="request">회원가입에 필요한 이메일, 비밀번호, PlayerId 등의 정보</param>
        /// <returns>회원가입 성공 시 생성된 사용자 ID</returns>
        /// <response code="200">회원가입이 성공적으로 완료됨</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), 200)]
        public async Task<ActionResult<BaseResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("회원가입 시도: {Email}, {PlayerId}", request.Email, request.PlayerId);

            await _registerUseCase.ExecuteAsync(
                new RegisterCommand(
                    request.PlayerId,
                    request.Email,
                    request.Password
                    )
                );

            _logger.LogInformation("회원가입 성공: PlayerId={PlayerId}", request.PlayerId);

            return Ok(ApiResponse.Ok<EmptyResponse>("회원가입 성공"));
        }

        /// <summary>
        /// 로그인 APi
        /// </summary>
        /// <param name="request">로그인에 필요한 PlayerId와 비밀번호</param>
        /// <returns>로그인 결과</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), 200)]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("로그인 시도: PlayerId={PlayerId}", request.PlayerId);

            var result = await _loginUseCase.ExecuteAsync(
                new LoginCommand(
                    request.PlayerId,
                    request.Password
                    )
                );

            var response = new LoginResponse
            {
                IsNewAccount = result.IsNewAccount
            };

            _logger.LogInformation($"로그인 성공: PlayerId={request.PlayerId}");
            
            return Ok(ApiResponse.Ok("로그인 성공", response));
        }

        /// <summary>
        /// 로그아웃 API
        /// </summary>
        /// <returns>로그아웃 결과</returns>
        [HttpPost("logout")]
        [SessionAuthorize]
        [ProducesResponseType(typeof(BaseResponse<EmptyResponse>), 200)]
        public async Task<ActionResult<BaseResponse<EmptyResponse>>> Logout()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;

            _logger.LogInformation("로그아웃 시도: {SessionId}", sessionId);

            await _logoutUseCase.ExecuteAsync(sessionId);

            _logger.LogInformation("로그아웃 성공: {SessionId}", sessionId);

            return Ok(ApiResponse.Ok<EmptyResponse>("로그아웃 성공"));
        }
    }
}
