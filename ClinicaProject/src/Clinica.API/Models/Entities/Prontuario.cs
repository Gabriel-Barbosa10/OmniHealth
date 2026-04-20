using System;

namespace Clinica.API.Models.Entities
{
    /// <summary>
    /// Entidade que representa a tabela 'prontuario' no banco de dados (clinica.db).
    /// Todas as validações espelham as constraints definidas no script SQL.
    /// </summary>
    public class Prontuario
    {
        // ── Propriedades (mapeiam colunas do banco) ──────────────────────────
        public int Id { get; private set; }
        public int IdPaciente { get; private set; }
        public int IdTerapeuta { get; private set; }
        public int Versao { get; private set; }
        public string? Descricao { get; private set; }

        // ── Construtor privado (impede criação sem validação) ────────────────
        private Prontuario() { }

        // ── Factory: criação de novo registro ────────────────────────────────
        /// <summary>
        /// Cria uma nova instância validada de Prontuario para INSERT no banco.
        /// Descricao é opcional conforme o schema SQL (sem NOT NULL).
        /// Versao inicia em 1 conforme DEFAULT do SQL.
        /// </summary>
        public static Prontuario Criar(int idPaciente, int idTerapeuta, string? descricao = null)
        {
            // 1. FK id_paciente — NOT NULL
            if (idPaciente <= 0)
                throw new ArgumentException("Id do paciente deve ser um valor positivo válido.");

            // 2. FK id_terapeuta — NOT NULL
            if (idTerapeuta <= 0)
                throw new ArgumentException("Id do terapeuta deve ser um valor positivo válido.");

            // 3. Descricao — NULLABLE, mas max 5000 se fornecida
            if (descricao != null)
            {
                descricao = descricao.Trim();
                if (descricao.Length == 0)
                    descricao = null; // Trata string vazia como null
                else if (descricao.Length > 5000)
                    throw new ArgumentException("Descrição deve ter no máximo 5000 caracteres.");
            }

            return new Prontuario
            {
                IdPaciente = idPaciente,
                IdTerapeuta = idTerapeuta,
                Versao = 1,
                Descricao = descricao
            };
        }

        // ── Factory: reconstrução a partir de dados do banco (SELECT) ────────
        /// <summary>
        /// Reconstrói a entidade a partir de uma linha do banco de dados.
        /// </summary>
        public static Prontuario FromDatabase(int id, int idPaciente, int idTerapeuta, int versao, string? descricao)
        {
            if (id <= 0)
                throw new InvalidOperationException("Id inválido ao reconstruir Prontuario do banco.");

            return new Prontuario
            {
                Id = id,
                IdPaciente = idPaciente,
                IdTerapeuta = idTerapeuta,
                Versao = versao,
                Descricao = descricao
            };
        }

        // ── Métodos de negócio ───────────────────────────────────────────────

        /// <summary>
        /// Atualiza a descrição do prontuário com validação.
        /// Incrementa a versão automaticamente.
        /// </summary>
        public void AtualizarDescricao(string? novaDescricao)
        {
            if (novaDescricao != null)
            {
                novaDescricao = novaDescricao.Trim();
                if (novaDescricao.Length == 0)
                    novaDescricao = null;
                else if (novaDescricao.Length > 5000)
                    throw new ArgumentException("Descrição deve ter no máximo 5000 caracteres.");
            }

            Descricao = novaDescricao;
            Versao++;
        }
    }
}