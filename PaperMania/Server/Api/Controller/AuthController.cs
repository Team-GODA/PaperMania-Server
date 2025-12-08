using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Auth;
using Server.Api.Filter;
using Server.Application.UseCase.Auth;
using Server.Application.UseCase.Auth.Command;  

namespace Server.Api.Controller
{
    [Route("api/v3/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRegisterUseCase _registerUseCase;
        private readonly ILoginUseCase _loginUseCase;
        private readonly ILogoutUseCase _logoutUseCase;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IRegisterUseCase registerUseCase,
            ILoginUseCase loginUseCase,
            ILogoutUseCase logoutUseCase,
            ILogger<AuthController> logger)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _logoutUseCase = logoutUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Session-Id를 통해 유효한 유저인지 확인합니다.
        /// </summary>
        /// <param name="request">유저 확인을 유한 header의 Session-Id의 값</param>
        /// <returns>유효한 유저 확인 후 해당 유저 ID와 성공 </returns>>
        [HttpPost("validate")]
        [ServiceFilter(typeof(SessionValidationFilter))]
        [ProducesResponseType(typeof(BaseResponse<ValidateUserResponse>), 200)]
        // public async Task<ActionResult<BaseResponse<ValidateUserResponse>>> ValidateUserBySessionId()
        // {
        //     var sessionId = HttpContext.Items["SessionId"] as string;
        //     
        //
        //     var response = new ValidateUserResponse
        //     {
        //         UserId = userId,
        //         IsValidated = true
        //     };
        //
        //     return Ok(ApiResponse.Ok("유저 인증 성공", response));
        // }
        
        /// <summary>
        /// 신규 회원가입을 처리합니다.
        /// </summary>
        /// <param name="request">회원가입에 필요한 이메일, 비밀번호, PlayerId 등의 정보</param>
        /// <returns>회원가입 성공 시 생성된 사용자 ID</returns>
        /// <response code="200">회원가입이 성공적으로 완료됨</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), 200)]
        public async Task<ActionResult<BaseResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("회원가입 시도: {Email}, {PlayerId}", request.Email, request.PlayerId);

            var response = await _registerUseCase.ExecuteAsync(
                new RegisterCommand(request.PlayerId, request.Email, request.Password)
            );

            _logger.LogInformation("회원가입 성공: {PlayerId}", request.PlayerId);

            return Ok(ApiResponse.Ok("회원가입 성공", response));
        }

        /// <summary>
        /// 로그인 요청을 처리합니다.
        /// </summary>
        /// <param name="request">로그인에 필요한 PlayerId와 비밀번호</param>
        /// <returns>로그인 결과</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), 200)]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("로그인 시도: PlayerId={PlayerId}", request.PlayerId);

            var response = await _loginUseCase.ExecuteAsync(
                new LoginCommand(request.PlayerId, request.Password)
                );

            _logger.LogInformation($"로그인 성공: PlayerId={request.PlayerId}");
            
            return Ok(ApiResponse.Ok("로그인 성공", response));
        }

        /// <summary>
        /// 구글 로그인 API
        /// </summary>
        /// <param name="request">구글 로그인 요청 정보</param>
        /// <returns>로그인 결과</returns>
        // [HttpPost("login/google")]
        // [ProducesResponseType(typeof(BaseResponse<GoogleLoginResponse>), 200)]
        // public async Task<ActionResult<BaseResponse<GoogleLoginResponse>>> LoginByGoogle([FromBody] GoogleLoginRequest request)
        // {
        //     _logger.LogInformation("구글 로그인 시도");
        //
        //     var sessionId = await _accountService.LoginByGoogleAsync(request.IdToken);
        //
        //     var response = new GoogleLoginResponse
        //     {
        //         SessionId = sessionId!,
        //     };
        //
        //     _logger.LogInformation("구글 로그인 성공");
        //     return Ok(ApiResponse.Ok("구글 로그인 성공", response));
        // }

        /// <summary>
        /// 로그아웃 API
        /// </summary>
        /// <returns>로그아웃 결과</returns>
        [HttpPost("logout")]
        [SessionAuthorize]
        [ProducesResponseType(typeof(BaseResponse<EmptyResponse>), 200)]
        public async Task<IActionResult> Logout()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;

            _logger.LogInformation("로그아웃 시도: {SessionId}", sessionId);

            await _logoutUseCase.ExecuteAsync(new LogoutCommand(sessionId));

            _logger.LogInformation("로그아웃 성공: {SessionId}", sessionId);

            return Ok(ApiResponse.Ok<EmptyResponse>("로그아웃 성공"));
        }
    }
}
