using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Auth;
using Server.Api.Filter;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAccountService accountService, 
            ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        
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
            _logger.LogInformation("회원가입 시도: Email={Email}, PlayerId={PlayerId}", request.Email, request.PlayerId);

            var newUser = new PlayerAccountData
            {
                PlayerId = request.PlayerId,
                Email = request.Email
            };

            await _accountService.RegisterAsync(newUser, request.Password);

            var response = new RegisterResponse
            {   
                Id = newUser.Id
            };

            _logger.LogInformation("회원가입 성공: UserId={UserId}, PlayerId={PlayerId}", response.Id, request.PlayerId);
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

            var (sessionId, user) = await _accountService.LoginAsync(request.PlayerId, request.Password);

            var response = new LoginResponse
            {
                IsNewAccount = user.IsNewAccount,
                SessionId = sessionId
            };

            _logger.LogInformation($"로그인 성공: PlayerId={request.PlayerId}");
            return Ok(ApiResponse.Ok("로그인 성공", response));
        }

        /// <summary>
        /// 구글 로그인 API
        /// </summary>
        /// <param name="request">구글 로그인 요청 정보</param>
        /// <returns>로그인 결과</returns>
        [HttpPost("login/google")]
        [ProducesResponseType(typeof(BaseResponse<GoogleLoginResponse>), 200)]
        public async Task<ActionResult<BaseResponse<GoogleLoginResponse>>> LoginByGoogle([FromBody] GoogleLoginRequest request)
        {
            _logger.LogInformation("구글 로그인 시도");

            var sessionId = await _accountService.LoginByGoogleAsync(request.IdToken);

            var response = new GoogleLoginResponse
            {
                SessionId = sessionId!,
            };

            _logger.LogInformation("구글 로그인 성공");
            return Ok(ApiResponse.Ok("구글 로그인 성공", response));
        }

        /// <summary>
        /// 로그아웃 API
        /// </summary>
        /// <returns>로그아웃 결과</returns>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(BaseResponse<EmptyResponse>), 200)]
        [ServiceFilter(typeof(SessionValidationFilter))]
        public async Task<ActionResult<BaseResponse<EmptyResponse>>> Logout()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            _logger.LogInformation("로그아웃 시도: SessionId={SessionId}", sessionId);

            await _accountService.LogoutAsync(sessionId!);

            _logger.LogInformation("로그아웃 성공: SessionId={SessionId}", sessionId);
            return Ok(ApiResponse.Ok<EmptyResponse>("로그아웃 성공"));
        }
    }
}
