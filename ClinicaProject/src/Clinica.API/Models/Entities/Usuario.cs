using System;

namespace Clinica.API.Models.Entities
{
    public class Usuario
    {
        public int      Id          { get; private set; }
        public string   Nome        { get; private set; } = string.Empty;
        public string   Email       { get; private set; } = string.Empty;
        public string   Cpf         { get; private set; } = string.Empty;
        public string   SenhaHash   { get; private set; } = string.Empty;
        public string   TipoPerfil  { get; private set; } = string.Empty;
        public bool     AceiteLgpd  { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public bool     Ativo       { get; private set; }

        private static readonly string[] PerfisPermitidos = { "PACIENTE", "TERAPEUTA" };

        private Usuario() { }

        public static Usuario Criar(string nome, string email, string cpf, string senhaHash, string tipoPerfil, bool aceiteLgpd)
        {
            if (!aceiteLgpd)
                throw new ArgumentException("É obrigatório aceitar os termos da LGPD.");

            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail é obrigatório.");

            tipoPerfil = tipoPerfil.Trim().ToUpperInvariant();
            if (Array.IndexOf(PerfisPermitidos, tipoPerfil) == -1)
                throw new ArgumentException("Tipo de perfil deve ser PACIENTE ou TERAPEUTA.");

            return new Usuario
            {
                Nome        = nome.Trim(),
                Email       = email.Trim().ToLowerInvariant(),
                Cpf         = cpf?.Trim() ?? string.Empty,
                SenhaHash   = senhaHash,
                TipoPerfil  = tipoPerfil,
                AceiteLgpd  = aceiteLgpd,
                DataCriacao = DateTime.UtcNow,
                Ativo       = true
            };
        }

        public static Usuario FromDatabase(int id, string nome, string email, string cpf,
            string senhaHash, string tipoPerfil, DateTime dataCriacao, bool ativo)
        {
            return new Usuario
            {
                Id          = id,
                Nome        = nome,
                Email       = email,
                Cpf         = cpf,
                SenhaHash   = senhaHash,
                TipoPerfil  = tipoPerfil,
                DataCriacao = dataCriacao,
                Ativo       = ativo
            };
        }
    }
}