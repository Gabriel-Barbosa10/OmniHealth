[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController() 
    {
        _authService = new AuthService(); 
    }

    [HttpPost("register")]
    public IActionResult Register(UserRegisterDTO model)
    {
        // A controller só faz uma pergunta simples ao Service
        var sucesso = _authService.RegistrarUsuario(model);

        if (!sucesso) return BadRequest("Dados inválidos.");

        return Ok("Usuário criado com sucesso!");
    }
}