using System;

namespace Clinica.API.Models.Entities
{
    /// <summary>
    /// Entidade que representa a tabela 'agendamento' no banco de dados (clinica.db).
    /// Todas as validações espelham as constraints definidas no script SQL.
    /// </summary>
    public class Agendamento
    {
        // ── Propriedades (mapeiam colunas do banco) ──────────────────────────
        public int Id { get; private set; }
        public int IdPaciente { get; private set; }
        public int IdTerapeuta { get; private set; }
        public string Status { get; private set; } = string.Empty;

        // ── Valores permitidos (espelham CHECK do SQL) ───────────────────────
        private static readonly string[] StatusPermitidos =
            { "PENDENTE", "CONFIRMADO", "CANCELADO", "REALIZADO", "NO_SHOW" };

        // ── Construtor privado (impede criação sem validação) ────────────────
        private Agendamento() { }

        // ── Factory: criação de novo registro ────────────────────────────────
        /// <summary>
        /// Cria uma nova instância validada de Agendamento para INSERT no banco.
        /// O status padrão é 'PENDENTE', conforme DEFAULT do SQL.
        /// </summary>
        public static Agendamento Criar(int idPaciente, int idTerapeuta, string status = "PENDENTE")
        {
            // 1. FK id_paciente — NOT NULL, deve referenciar um usuario válido
            if (idPaciente <= 0)
                throw new ArgumentException("Id do paciente deve ser um valor positivo válido.");

            // 2. FK id_terapeuta — NOT NULL, deve referenciar um usuario válido
            if (idTerapeuta <= 0)
                throw new ArgumentException("Id do terapeuta deve ser um valor positivo válido.");

            // 3. Paciente e terapeuta não podem ser a mesma pessoa
            if (idPaciente == idTerapeuta)
                throw new ArgumentException("Paciente e terapeuta não podem ser o mesmo usuário.");

            // 4. Status — NOT NULL, CHECK IN (...)
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status é obrigatório.");
            status = status.Trim().ToUpperInvariant();
            if (Array.IndexOf(StatusPermitidos, status) == -1)
                throw new ArgumentException($"Status inválido. Valores permitidos: {string.Join(", ", StatusPermitidos)}.");

            return new Agendamento
            {
                IdPaciente = idPaciente,
                IdTerapeuta = idTerapeuta,
                Status = status
            };
        }

        // ── Factory: reconstrução a partir de dados do banco (SELECT) ────────
        /// <summary>
        /// Reconstrói a entidade a partir de uma linha do banco de dados.
        /// </summary>
        public static Agendamento FromDatabase(int id, int idPaciente, int idTerapeuta, string status)
        {
            if (id <= 0)
                throw new InvalidOperationException("Id inválido ao reconstruir Agendamento do banco.");

            return new Agendamento
            {
                Id = id,
                IdPaciente = idPaciente,
                IdTerapeuta = idTerapeuta,
                Status = status ?? "PENDENTE"
            };
        }

        // ── Método de negócio: alteração de status ───────────────────────────
        /// <summary>
        /// Atualiza o status do agendamento com validação.
        /// Útil antes de executar um UPDATE no banco.
        /// </summary>
        public void AlterarStatus(string novoStatus)
        {
            if (string.IsNullOrWhiteSpace(novoStatus))
                throw new ArgumentException("Novo status é obrigatório.");
            novoStatus = novoStatus.Trim().ToUpperInvariant();
            if (Array.IndexOf(StatusPermitidos, novoStatus) == -1)
                throw new ArgumentException($"Status inválido. Valores permitidos: {string.Join(", ", StatusPermitidos)}.");

            Status = novoStatus;
        }
    }
}