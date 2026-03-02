using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobScoreServer.DTOs;
using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.Login(request.email, request.password);

                if (token == null)
                {
                    return Unauthorized(new { error = "Email or password are incorrect" });
                }

                SetAuthCookie(token);

                return Ok(new { message = "You've logged in successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOs.RegisterRequest request)
        {
            try
            {
                var token = await _authService.Register(request);

                SetAuthCookie(token);

                return Ok(new { message = "You've registered successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private void SetAuthCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            };

            Response.Cookies.Append("jwt_token", token, cookieOptions);
        }
    }
}
