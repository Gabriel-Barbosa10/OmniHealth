using System;

namespace Clinica.API.Models.Entities
{
    /// <summary>
    /// Entidade que representa a tabela 'nota_evolucao' no banco de dados (clinica.db).
    /// Todas as validações espelham as constraints definidas no script SQL.
    /// </summary>
    public class NotaEvolucao
    {
        // ── Propriedades (mapeiam colunas do banco) ──────────────────────────
        public int Id { get; private set; }
        public int IdProntuario { get; private set; }
        public int IdTerapeuta { get; private set; }
        public int? IdAgendamento { get; private set; }
        public string TextoEvolucao { get; private set; } = string.Empty;
        public DateTime DataRegistro { get; private set; }

        // ── Construtor privado (impede criação sem validação) ────────────────
        private NotaEvolucao() { }

        // ── Factory: criação de novo registro ────────────────────────────────
        /// <summary>
        /// Cria uma nova instância validada de NotaEvolucao para INSERT no banco.
        /// DataRegistro é preenchido automaticamente com DateTime.UtcNow
        /// (espelha o DEFAULT CURRENT_TIMESTAMP do SQL).
        /// </summary>
        public static NotaEvolucao Criar(int idProntuario, int idTerapeuta, string textoEvolucao, int? idAgendamento = null)
        {
            // 1. FK id_prontuario — NOT NULL
            if (idProntuario <= 0)
                throw new ArgumentException("Id do prontuário deve ser um valor positivo válido.");

            // 2. FK id_terapeuta — NOT NULL
            if (idTerapeuta <= 0)
                throw new ArgumentException("Id do terapeuta deve ser um valor positivo válido.");

            // 3. FK id_agendamento — NULLABLE, mas se fornecido deve ser válido
            if (idAgendamento.HasValue && idAgendamento.Value <= 0)
                throw new ArgumentException("Id do agendamento, quando informado, deve ser um valor positivo válido.");

            // 4. TextoEvolucao — NOT NULL, max 5000
            if (string.IsNullOrWhiteSpace(textoEvolucao))
                throw new ArgumentException("Texto da evolução é obrigatório.");
            textoEvolucao = textoEvolucao.Trim();
            if (textoEvolucao.Length > 5000)
                throw new ArgumentException("Texto da evolução deve ter no máximo 5000 caracteres.");

            return new NotaEvolucao
            {
                IdProntuario = idProntuario,
                IdTerapeuta = idTerapeuta,
                IdAgendamento = idAgendamento,
                TextoEvolucao = textoEvolucao,
                DataRegistro = DateTime.UtcNow
            };
        }

        // ── Factory: reconstrução a partir de dados do banco (SELECT) ────────
        /// <summary>
        /// Reconstrói a entidade a partir de uma linha do banco de dados.
        /// </summary>
        public static NotaEvolucao FromDatabase(int id, int idProntuario, int idTerapeuta, int? idAgendamento, string textoEvolucao, DateTime dataRegistro)
        {
            if (id <= 0)
                throw new InvalidOperationException("Id inválido ao reconstruir NotaEvolucao do banco.");

            return new NotaEvolucao
            {
                Id = id,
                IdProntuario = idProntuario,
                IdTerapeuta = idTerapeuta,
                IdAgendamento = idAgendamento,
                TextoEvolucao = textoEvolucao ?? string.Empty,
                DataRegistro = dataRegistro
            };
        }
    }
}