using JobScoreServer.DTOs;
using JobScoreServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

                return Ok(new 
                { 
                    message = "You've logged in successfully!",
                    token = token
                });
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

                return Ok(new 
                { 
                    message = "You've registered successfully!",
                    token = token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
            var isAdmin = User.FindFirst("IsAdmin")?.Value == "True";

            return Ok(new
            {
                id = userId,
                email = email,
                firstName = firstName,
                lastName = lastName,
                isAdmin = isAdmin
            });
        }
    }
}
