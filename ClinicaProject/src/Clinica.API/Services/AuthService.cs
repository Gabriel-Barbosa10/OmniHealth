using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.API.Models.DTOs;
using Clinica.API.Models.Entities;
using Clinica.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Clinica.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IConfiguration _config;

        public AuthService(IUsuarioRepository usuarioRepo, IConfiguration config)
        {
            _usuarioRepo = usuarioRepo;
            _config = config;
        }

        public async Task<bool> CadastrarUsuarioAsync(UserRegisterDTO dados)
        {
            if (string.IsNullOrWhiteSpace(dados.Nome)) throw new ArgumentException("Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(dados.Email)) throw new ArgumentException("E-mail é obrigatório.");
            if (string.IsNullOrWhiteSpace(dados.Password) || dados.Password.Length < 8)
                throw new ArgumentException("A senha deve ter no mínimo 8 caracteres.");

            var usuarioExistente = await _usuarioRepo.BuscarPorEmail(dados.Email);
            if (usuarioExistente != null)
                throw new InvalidOperationException("Este e-mail já está cadastrado.");

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(dados.Password, workFactor: 12);

            var novoUsuario = Usuario.Criar(
                nome: dados.Nome,
                email: dados.Email,
                cpf: dados.Cpf ?? string.Empty,
                senhaHash: senhaHash,
                tipoPerfil: dados.TipoPerfil ?? "PACIENTE",
                aceiteLgpd: dados.AceiteLgpd
            );

            return await _usuarioRepo.CadastrarUsuario(novoUsuario);
        }

        public async Task<string?> LoginAsync(UserLoginDTO request)
        {
            var usuario = await _usuarioRepo.BuscarPorEmail(request.Email);
            if (usuario == null) return null;

            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(request.Password, usuario.SenhaHash);
            if (!senhaCorreta) return null;

            return GerarTokenJwt(usuario);
        }

        private string GerarTokenJwt(Usuario usuario)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var chaveSecreta = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("Chave JWT não configurada em appsettings.");

            var chaveBytes = Encoding.UTF8.GetBytes(chaveSecreta);
            var credenciais = new SigningCredentials(
                new SymmetricSecurityKey(chaveBytes),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.TipoPerfil),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            int expiracaoHoras = int.TryParse(jwtSettings["ExpirationHours"], out var h) ? h : 8;

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"] ?? "ClinicaAPI",
                audience: jwtSettings["Audience"] ?? "ClinicaApp",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiracaoHoras),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}