using Clinica.API.Models.DTOs;
using System.Threading.Tasks;

namespace Clinica.API.Services
{
    public interface IAuthService
    {
        Task<bool> CadastrarUsuarioAsync(UserRegisterDTO dados);
        Task<string?> LoginAsync(UserLoginDTO request);
    }
}
