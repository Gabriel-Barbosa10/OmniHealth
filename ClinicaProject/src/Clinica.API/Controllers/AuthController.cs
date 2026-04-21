using Clinica.API.Models.DTOs;
using Clinica.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
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
            var token = await _authService.LoginAsync(request);

            if (token is null)
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

            return Ok(new
            {
                token,
                tipo = "Bearer"
            });
        }
    }
}