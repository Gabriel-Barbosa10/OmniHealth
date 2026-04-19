namespace Clinica.API.Services
{
    public class AuthService
    {
        // Esta função só tem UMA responsabilidade: a lógica de negócio do cadastro
        public bool RegistrarUsuario(UserRegisterDTO dados)
        {
            // 1. Lógica: O CPF é válido?
            if (dados.Cpf.Length != 11) return false;

            // 2. Lógica: A senha tem o tamanho mínimo?
            if (dados.Password.Length < 6) return false;

            // 3. Se tudo estiver OK, ele manda salvar (via Repository ou Context)
            // ... lógica de salvar aqui ...
            
            return true;
        }
    }
}