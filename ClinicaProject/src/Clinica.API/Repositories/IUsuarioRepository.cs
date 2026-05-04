using Clinica.API.Models.Entities;

namespace Clinica.API.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> BuscarPorEmail(string email);
        Task<bool> CadastrarUsuario(Usuario usuario);
    }
}