using Clinica.API.Models.DTOs;
 
namespace Clinica.API.Services
{
    public interface IAuthService
    {
        bool    CadastrarUsuario(UserRegisterDTO dados);
        string? Login(string email, string senha);
    }
}