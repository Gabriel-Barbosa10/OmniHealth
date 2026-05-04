using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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

            // 4. CPF — NOT NULL, UNIQUE, max 14
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF é obrigatório.");
            cpf = cpf.Trim();
            if (cpf.Length > 14)
                throw new ArgumentException("CPF deve ter no máximo 14 caracteres.");

            if(ValidarCpf(cpf) is false)
            {
                throw new ArgumentException("CPF inválido");
            }
            // 5. SenhaHash — NOT NULL
            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha criptografada é obrigatória.");

            // 6. TipoPerfil — CHECK IN ('PACIENTE', 'TERAPEUTA')
            if (string.IsNullOrWhiteSpace(tipoPerfil))
                throw new ArgumentException("Tipo de perfil é obrigatório.");
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
            string senhaHash, string tipoPerfil, DateTime dataCriacao, bool ativo, bool aceiteLgpd)
        {
            return new Usuario
            {
                Id          = id,
                Nome        = nome,
                Email       = email,
                Cpf         = cpf,
                SenhaHash   = senhaHash,
                TipoPerfil  = tipoPerfil,
                AceiteLgpd  = aceiteLgpd,
                DataCriacao = dataCriacao,
                Ativo       = ativo
            };
        }

        // ── Validação de CPF (dígitos verificadores) ─────────────────────────
        /// <summary>
        /// Valida CPF com ou sem máscara (ex: 123.456.789-09 ou 12345678909).
        /// </summary>

        public static bool ValidarCpf(string cpf) 
        {
            cpf=cpf.Trim();
            cpf=cpf.Replace(".","").Replace("-","");
            string digitosCpf=cpf.Substring(0,9);
            int[]multiplicadores1=new int[9]{10,9,8,7,6,5,4,3,2};
            int[]multiplicadores2=new int[10]{11,10,9,8,7,6,5,4,3,2};
            int primeiroDigito=0,segundoDigito=0;
            

            //Verificando se todos os dígitos são iguais
            bool verificarDigitosIguais=cpf.All(c=>c==cpf[0]);

            if(verificarDigitosIguais is true)
            {
                return false;
            }

            //Validando primeiro dígito verificador
            
            for(int contador=0; contador<9; contador++)
            {
                primeiroDigito+=(multiplicadores1[contador]*Convert.ToInt16(digitosCpf[contador].ToString()));
            }

            primeiroDigito=11-primeiroDigito%11;
            if(primeiroDigito>=10)
            primeiroDigito=0;

            digitosCpf+=primeiroDigito.ToString();
            
            //Validando segundo dígito verificador

            for(int contador=0;contador<10;contador++){
                segundoDigito+=(multiplicadores2[contador]*Convert.ToInt16(digitosCpf[contador].ToString()));
            }
            segundoDigito=11-segundoDigito%11;

            if (segundoDigito >= 10)
            {
                segundoDigito=0;
            }
            digitosCpf+=segundoDigito.ToString();

            if (digitosCpf.Equals(cpf))
                return true;

            else
            return false;


        }
    }
}