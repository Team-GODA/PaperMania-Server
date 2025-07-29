using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Auth;
using Server.Api.Filter;
using Server.Application.Exceptions;
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
        private readonly ISessionService _sessionService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAccountService accountService, ISessionService sessionService, 
            ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _sessionService =  sessionService;
            _logger = logger;
        }

        
        /// <summary>
        /// 신규 회원가입을 처리합니다.
        /// </summary>
        /// <param name="request">회원가입에 필요한 이메일, 비밀번호, PlayerId 등의 정보</param>
        /// <returns>회원가입 성공 시 생성된 사용자 ID</returns>
        /// <response code="200">회원가입이 성공적으로 완료됨</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("회원가입 시도: Email={Email}, PlayerId={PlayerId}", request.Email, request.PlayerId);

            try
            {
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

                _logger.LogInformation("회원가입 성공: Id={Id}, PlayerId={PlayerId}", response.Id, request.PlayerId);
                return Ok(ApiResponse.Ok("회원가입 성공", response));
            }
            catch (DuplicateEmailException ex)
            {
                _logger.LogWarning(ex, "회원가입 실패: 이메일 중복: {Email}", request.Email);
                return Ok(ApiResponse.Error<RegisterResponse>((int)ErrorStatusCode.Conflict, ex.Message));
            }
            catch (DuplicatePlayerIdException ex)
            {
                _logger.LogWarning(ex, "회원가입 실패: PlayerId 중복: {PlayerId}", request.PlayerId);
                return Ok(ApiResponse.Error<RegisterResponse>((int)ErrorStatusCode.Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류:  회원가입 중 예외 발생");
                return Ok(ApiResponse.Error<RegisterResponse>((int)ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 로그인 요청을 처리합니다.
        /// </summary>
        /// <param name="request">로그인에 필요한 PlayerId와 비밀번호</param>
        /// <returns>로그인 결과</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("로그인 시도: PlayerId={PlayerId}", request.PlayerId);

            try
            {
                var (sessionId, user) = await _accountService.LoginAsync(request.PlayerId, request.Password);

                var response = new LoginResponse
                {
                    Id = user.Id,
                    SessionId = sessionId
                };

                _logger.LogInformation($"로그인 성공: PlayerId={request.PlayerId}");
                return Ok(ApiResponse.Ok("로그인 성공", response));
            }
            catch (AuthenticationFailedException ex)
            {
                _logger.LogWarning(ex, $"로그인 실패: 아이디 또는 비밀번호 불일치 : PlayerId={request.PlayerId}");
                return Ok(ApiResponse.Error<LoginResponse>((int)ErrorStatusCode.BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류: 로그인 중 예외 발생");
                return Ok(ApiResponse.Error<RegisterResponse>((int)ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 구글 로그인 API
        /// </summary>
        /// <param name="request">구글 로그인 요청 정보</param>
        /// <returns>로그인 결과</returns>
        [HttpPost("login/google")]
        [ProducesResponseType(typeof(GoogleLoginResponse), 200)]
        public async Task<ActionResult<GoogleLoginResponse>> LoginByGoogle([FromBody] GoogleLoginRequest request)
        {
            _logger.LogInformation("구글 로그인 시도");

            try
            {
                var sessionId = await _accountService.LoginByGoogleAsync(request.IdToken);

                var response = new GoogleLoginResponse
                {
                    SessionId = sessionId,
                };

                _logger.LogInformation("구글 로그인 성공");
                return Ok(ApiResponse.Ok("구글 로그인 성공", response));
            }
            catch (GoogleLoginFailedException ex)
            {
                _logger.LogWarning(ex, "구글 로그인 실패");
                return Ok(ApiResponse.Error<GoogleLoginResponse>((int)ErrorStatusCode.Unauthorized, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류: 구글 로그인 중 예외 발생");
                return Ok(ApiResponse.Error<RegisterResponse>((int)ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 로그아웃 API
        /// </summary>
        /// <returns>로그아웃 결과</returns>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(LogoutResponse), 200)]
        [ServiceFilter(typeof(SessionValidationFilter))]
        public async Task<ActionResult<LogoutResponse>> Logout()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId =  await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation("로그아웃 시도: SessionId={SessionId}", sessionId);

            try
            {
                await _accountService.LogoutAsync(sessionId);

                _logger.LogInformation("로그아웃 성공: SessionId={SessionId}", sessionId);
                return Ok(ApiResponse.Ok<LogoutResponse>("로그아웃 성공"));
            }
            catch (SessionNotFoundException ex)
            {
                _logger.LogWarning(ex, "로그아웃 실패: 세션 ID 없음");
                return Ok(ApiResponse.Error<LogoutResponse>((int)ErrorStatusCode.Unauthorized, "SID가 없습니다."));
            }
            catch (SessionValidationException ex)
            {
                _logger.LogWarning(ex, "로그아웃 실패:  유효하지 않은 세션: SessionId={SessionId}", sessionId);
                return Ok(ApiResponse.Error<LogoutResponse>((int)ErrorStatusCode.Unauthorized, "유효하지 않는 SID 입니다"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류 - 로그아웃 중 예외 발생");
                return Ok(ApiResponse.Error<RegisterResponse>((int)ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
        }
    }
}
