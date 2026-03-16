// Controllers/AuthController.cs
using DoConnect.API.DTOs;
using DoConnect.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) { _authService = authService; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });

            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Username or email already taken." });

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input." });

            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result);
        }
    }
}
