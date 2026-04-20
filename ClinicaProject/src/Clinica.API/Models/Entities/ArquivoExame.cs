using System;

namespace Clinica.API.Models.Entities
{
    /// <summary>
    /// Entidade que representa a tabela 'arquivo_exame' no banco de dados (clinica.db).
    /// Todas as validações espelham as constraints definidas no script SQL.
    /// </summary>
    public class ArquivoExame
    {
        // ── Propriedades (mapeiam colunas do banco) ──────────────────────────
        public int Id { get; private set; }
        public string CaminhoStorage { get; private set; } = string.Empty;
        public long? TamanhoBytes { get; private set; }
        public string? NomeArquivo { get; private set; }
        public string? TipoMime { get; private set; }
        public DateTime DataUpload { get; private set; }
        public int? IdProntuario { get; private set; }

        // ── MIME types seguros permitidos ─────────────────────────────────────
        private static readonly string[] MimesPermitidos =
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif",
            "application/dicom"
        };

        // ── Construtor privado (impede criação sem validação) ────────────────
        private ArquivoExame() { }

        // ── Factory: criação de novo registro ────────────────────────────────
        /// <summary>
        /// Cria uma nova instância validada de ArquivoExame para INSERT no banco.
        /// DataUpload é preenchido automaticamente com DateTime.UtcNow
        /// (espelha o DEFAULT CURRENT_TIMESTAMP do SQL).
        /// </summary>
        public static ArquivoExame Criar(
            string caminhoStorage,
            long? tamanhoBytes = null,
            string? nomeArquivo = null,
            string? tipoMime = null,
            int? idProntuario = null)
        {
            // 1. CaminhoStorage — NOT NULL, max 500
            if (string.IsNullOrWhiteSpace(caminhoStorage))
                throw new ArgumentException("Caminho do storage é obrigatório.");
            caminhoStorage = caminhoStorage.Trim();
            if (caminhoStorage.Length > 500)
                throw new ArgumentException("Caminho do storage deve ter no máximo 500 caracteres.");

            // 2. TamanhoBytes — NULLABLE, mas se fornecido deve ser positivo
            if (tamanhoBytes.HasValue && tamanhoBytes.Value < 0)
                throw new ArgumentException("O tamanho do arquivo não pode ser negativo.");

            // 3. NomeArquivo — NULLABLE, max 255
            if (nomeArquivo != null)
            {
                nomeArquivo = nomeArquivo.Trim();
                if (nomeArquivo.Length == 0)
                    nomeArquivo = null;
                else if (nomeArquivo.Length > 255)
                    throw new ArgumentException("Nome do arquivo deve ter no máximo 255 caracteres.");
            }

            // 4. TipoMime — NULLABLE, validação de tipo permitido (segurança)
            if (tipoMime != null)
            {
                tipoMime = tipoMime.Trim().ToLowerInvariant();
                if (tipoMime.Length == 0)
                    tipoMime = null;
                else if (Array.IndexOf(MimesPermitidos, tipoMime) == -1)
                    throw new ArgumentException($"Tipo MIME não permitido. Valores aceitos: {string.Join(", ", MimesPermitidos)}.");
            }

            // 5. FK id_prontuario — NULLABLE, mas se fornecido deve ser válido
            if (idProntuario.HasValue && idProntuario.Value <= 0)
                throw new ArgumentException("Id do prontuário, quando informado, deve ser um valor positivo válido.");

            return new ArquivoExame
            {
                CaminhoStorage = caminhoStorage,
                TamanhoBytes = tamanhoBytes,
                NomeArquivo = nomeArquivo,
                TipoMime = tipoMime,
                DataUpload = DateTime.UtcNow,
                IdProntuario = idProntuario
            };
        }

        // ── Factory: reconstrução a partir de dados do banco (SELECT) ────────
        /// <summary>
        /// Reconstrói a entidade a partir de uma linha do banco de dados.
        /// </summary>
        public static ArquivoExame FromDatabase(
            int id,
            string caminhoStorage,
            long? tamanhoBytes,
            string? nomeArquivo,
            string? tipoMime,
            DateTime dataUpload,
            int? idProntuario)
        {
            if (id <= 0)
                throw new InvalidOperationException("Id inválido ao reconstruir ArquivoExame do banco.");

            return new ArquivoExame
            {
                Id = id,
                CaminhoStorage = caminhoStorage ?? string.Empty,
                TamanhoBytes = tamanhoBytes,
                NomeArquivo = nomeArquivo,
                TipoMime = tipoMime,
                DataUpload = dataUpload,
                IdProntuario = idProntuario
            };
        }
    }
}