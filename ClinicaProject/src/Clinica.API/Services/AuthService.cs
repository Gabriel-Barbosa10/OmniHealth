using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.API.Models.DTOs;
using Clinica.API.Models.Entities;
using Clinica.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Clinica.API.Services
{
    /// <summary>
    /// Contém a lógica de negócio de autenticação:
    /// criptografia de senha com BCrypt e geração de token JWT.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AuthRepository _authRepository;
        private readonly IConfiguration _config;

        public AuthService(AuthRepository authRepository, IConfiguration config)
        {
            _authRepository = authRepository;
            _config         = config;
        }

        // ── CADASTRO ─────────────────────────────────────────────────────────

        /// <summary>
        /// Regra de negócio do cadastro:
        /// 1. Valida os dados básicos.
        /// 2. Verifica se o e-mail já está em uso.
        /// 3. Criptografa a senha com BCrypt.
        /// 4. Persiste via Repository.
        /// </summary>
        public bool CadastrarUsuario(UserRegisterDTO dados)
        {
            // 1. Validações básicas
            if (string.IsNullOrWhiteSpace(dados.Nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(dados.Email))
                throw new ArgumentException("E-mail é obrigatório.");

            if (string.IsNullOrWhiteSpace(dados.Password) || dados.Password.Length < 8)
                throw new ArgumentException("A senha deve ter no mínimo 8 caracteres.");

            // 2. E-mail já cadastrado?
            var usuarioExistente = _authRepository.BuscarPorEmail(dados.Email);
            if (usuarioExistente != null)
                throw new InvalidOperationException("Este e-mail já está cadastrado.");

            // 3. Criptografar a senha com BCrypt (work factor 12)
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(dados.Password, workFactor: 12);

            // 4. Criar entidade e persistir
            var novoUsuario = Usuario.Criar(
                nome:      dados.Nome,
                email:     dados.Email,
                cpf:       dados.Cpf ?? string.Empty,
                senhaHash: senhaHash,
                tipoPerfil: dados.TipoPerfil ?? "PACIENTE",
                aceiteLgpd: dados.AceiteLgpd 
            );

            return _authRepository.CadastrarUsuario(novoUsuario);
        }

        // ── LOGIN ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Fluxo de login:
        /// 1. Localiza o usuário pelo e-mail (somente ativos).
        /// 2. Compara a senha informada com o hash armazenado via BCrypt.
        /// 3. Se bater, gera e retorna um token JWT.
        /// Retorna null se as credenciais forem inválidas.
        /// </summary>
        public string? Login(string email, string senha)
        {
            // 1. Buscar usuário ativo pelo e-mail
            var usuario = _authRepository.BuscarPorEmail(email);
            if (usuario == null)
                return null; // usuário não encontrado ou inativo

            // 2. Comparar senha com o hash usando BCrypt
            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
            if (!senhaCorreta)
                return null; // senha incorreta

            // 3. Credenciais válidas → gerar JWT
            return GerarTokenJwt(usuario);
        }

        // ── GERAÇÃO DE TOKEN JWT ──────────────────────────────────────────────

        private string GerarTokenJwt(Usuario usuario)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var chaveSecreta = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("Chave JWT não configurada em appsettings.");

            var chaveBytes  = Encoding.UTF8.GetBytes(chaveSecreta);
            var credenciais = new SigningCredentials(
                new SymmetricSecurityKey(chaveBytes),
                SecurityAlgorithms.HmacSha256
            );

            // Claims que serão incorporados ao token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Role,               usuario.TipoPerfil),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            int expiracaoHoras = int.TryParse(jwtSettings["ExpirationHours"], out var h) ? h : 8;

            var token = new JwtSecurityToken(
                issuer:   jwtSettings["Issuer"]   ?? "ClinicaAPI",
                audience: jwtSettings["Audience"] ?? "ClinicaApp",
                claims:   claims,
                expires:  DateTime.UtcNow.AddHours(expiracaoHoras),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}