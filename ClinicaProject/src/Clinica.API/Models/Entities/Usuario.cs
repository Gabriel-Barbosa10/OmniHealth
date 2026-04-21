using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Clinica.API.Models.Entities
{
    /// <summary>
    /// Entidade que representa a tabela 'usuario' no banco de dados (clinica.db).
    /// Todas as validações espelham as constraints definidas no script SQL.
    /// </summary>
    public class Usuario
    {
        // ── Propriedades (mapeiam colunas do banco) ──────────────────────────
        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Cpf { get; private set; } = string.Empty;
        public string Senha { get; private set; } = string.Empty;
        public string TipoPerfil { get; private set; } = string.Empty;
        public bool AceiteLgpd { get; private set; }

        // ── Valores permitidos (espelham CHECK do SQL) ───────────────────────
        private static readonly string[] PerfisPermitidos = { "PACIENTE", "TERAPEUTA" };

        // ── Construtor privado (impede criação sem validação) ────────────────
        private Usuario() { }

        // ── Factory: criação de novo registro ────────────────────────────────
        /// <summary>
        /// Cria uma nova instância validada de Usuario para INSERT no banco.
        /// </summary>
        public static Usuario Criar(string nome, string email, string cpf, string senha, string tipoPerfil, bool aceiteLgpd)
        {
            // 1. LGPD — obrigatório aceitar
            if (!aceiteLgpd)
                throw new ArgumentException("É obrigatório aceitar os termos da LGPD para criar um usuário.");

            // 2. Nome — NOT NULL, max 100
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório.");
            nome = nome.Trim();
            if (nome.Length > 100)
                throw new ArgumentException("Nome deve ter no máximo 100 caracteres.");

            // 3. Email — NOT NULL, UNIQUE, max 255, formato válido
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório.");
            email = email.Trim().ToLowerInvariant();
            if (email.Length > 255)
                throw new ArgumentException("Email deve ter no máximo 255 caracteres.");
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("O formato do e-mail é inválido.");

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
            // 5. Senha — NOT NULL, min 8, max 20
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha é obrigatória.");
            if (senha.Length < 8 || senha.Length > 20)
                throw new ArgumentException("A senha deve ter entre 8 e 20 caracteres.");

            // 6. TipoPerfil — CHECK IN ('PACIENTE', 'TERAPEUTA')
            if (string.IsNullOrWhiteSpace(tipoPerfil))
                throw new ArgumentException("Tipo de perfil é obrigatório.");
            tipoPerfil = tipoPerfil.Trim().ToUpperInvariant();
            if (Array.IndexOf(PerfisPermitidos, tipoPerfil) == -1)
                throw new ArgumentException("O perfil deve ser PACIENTE ou TERAPEUTA.");

            return new Usuario
            {
                Nome = nome,
                Email = email,
                Cpf = cpf,
                Senha = senha,
                TipoPerfil = tipoPerfil,
                AceiteLgpd = aceiteLgpd
            };
        }

        // ── Factory: reconstrução a partir de dados do banco (SELECT) ────────
        /// <summary>
        /// Reconstrói a entidade a partir de uma linha do banco de dados.
        /// Não re-executa validações de negócio — o dado já foi validado no INSERT.
        /// </summary>
        public static Usuario FromDatabase(int id, string nome, string email, string cpf, string senha, string tipoPerfil, int aceiteLgpd)
        {
            if (id <= 0)
                throw new InvalidOperationException("Id inválido ao reconstruir Usuario do banco.");

            return new Usuario
            {
                Id = id,
                Nome = nome ?? string.Empty,
                Email = email ?? string.Empty,
                Cpf = cpf ?? string.Empty,
                Senha = senha ?? string.Empty,
                TipoPerfil = tipoPerfil ?? string.Empty,
                AceiteLgpd = aceiteLgpd == 1
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