using Clinica.API.Models.DTOs;
using Clinica.API.Services;
using Microsoft.AspNetCore.Mvc;
 
namespace Clinica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
 
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
 
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDTO model)
        {
            try
            {
                var sucesso = _authService.CadastrarUsuario(model);
                if (!sucesso) return BadRequest("Não foi possível criar o usuário.");
                return Ok("Usuário criado com sucesso!");
            }
            catch (ArgumentException ex)         { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }
 
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO model)
        {
            var token = _authService.Login(model.Email, model.Password);
            if (token == null) return Unauthorized("E-mail ou senha inválidos.");
            return Ok(new { token });
        }
    }
}