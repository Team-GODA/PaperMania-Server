using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Auth;
using Server.Application.Port.Input.Auth;
using Server.Application.UseCase.Auth.Command;

namespace Server.Api.Controller.Auth
{
    [ApiLog("Auth")]
    [Route("api/v3/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly ILoginUseCase _loginUseCase;
        private readonly IRegisterUseCase _registerUseCase;
        private readonly ILogoutUseCase _logoutUseCase;
        private readonly IValidateUseCase _validateUseCase;

        public AuthController(
            ILoginUseCase loginUseCase,
            IRegisterUseCase registerUseCase,
            ILogoutUseCase logoutUseCase,
            IValidateUseCase validateUseCase)
        {
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
            _logoutUseCase = logoutUseCase;
            _validateUseCase = validateUseCase;
        }

        /// <summary>
        /// 세션 유효 인증 API
        /// </summary>
        [HttpPost("validate")]
        [SessionAuthorize]
        public async Task<ActionResult<BaseResponse<ValidateUserResponse>>> ValidateUser()
        {
           await _validateUseCase.ExecuteAsync(SessionId);
           return Ok(ApiResponse.Ok<EmptyResponse>("유저 인증 성공"));
        }
        
        /// <summary>
        /// 신규 회원가입 API
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<BaseResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
        {
            await _registerUseCase.ExecuteAsync(
                new RegisterCommand(
                    request.PlayerId,
                    request.Email,
                    request.Password
                    )
                );

            return Ok(ApiResponse.Ok<EmptyResponse>("회원가입 성공"));
        }

        /// <summary>
        /// 로그인 APi
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            var result = await _loginUseCase.ExecuteAsync(
                new LoginCommand(
                    request.PlayerId,
                    request.Password
                    )
                );

            var response = new LoginResponse
            {
                SessionId = result.SessionId,
                IsNewAccount = result.IsNewAccount
            };

            return Ok(ApiResponse.Ok("로그인 성공", response));
        }

        /// <summary>
        /// 로그아웃 API
        /// </summary>
        [SessionAuthorize]
        [HttpPost("logout")]
        public async Task<ActionResult<BaseResponse<EmptyResponse>>> Logout()
        {
            await _logoutUseCase.ExecuteAsync(SessionId);
            return Ok(ApiResponse.Ok<EmptyResponse>("로그아웃 성공"));
        }
    }
}
