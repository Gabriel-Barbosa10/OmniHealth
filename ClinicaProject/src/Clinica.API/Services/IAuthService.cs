using Clinica.API.Models.DTOs;

namespace Clinica.API.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequest request);
    }
}