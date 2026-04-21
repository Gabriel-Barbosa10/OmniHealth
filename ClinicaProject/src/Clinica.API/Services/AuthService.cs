using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.API.Models.DTOs;
using Clinica.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Clinica.API.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IConfiguration     _config;

        public AuthService(IUsuarioRepository usuarioRepo, IConfiguration config)
        {
            _usuarioRepo = usuarioRepo;
            _config      = config;
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            // 1. Busca o usuário pelo e-mail
            var usuario = await _usuarioRepo.BuscarPorEmail(request.Email);
            if (usuario is null)
                return null;

            // 2. Compara a senha com o hash BCrypt
            bool senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha);
            if (!senhaValida)
                return null;

            // 3. Gera o token JWT
            return GerarToken(usuario.Id, usuario.Email, usuario.TipoPerfil);
        }

        private string GerarToken(int id, string email, string tipoPerfil)
        {
            var jwtSection = _config.GetSection("Jwt");
            var secretKey  = jwtSection["SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey não configurada.");

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role,               tipoPerfil),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer:             jwtSection["Issuer"],
                audience:           jwtSection["Audience"],
                claims:             claims,
                expires:            DateTime.UtcNow.AddMinutes(
                                        int.Parse(jwtSection["ExpiresInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}