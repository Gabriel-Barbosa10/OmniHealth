using System.Data;
using Clinica.API.Models.Entities;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Clinica.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClinicaDb")
                ?? throw new InvalidOperationException("Connection string 'ClinicaDb' não encontrada.");
        }

        public async Task<Usuario?> BuscarPorEmail(string email)
        {
            const string sql = """
                SELECT id, nome, email, cpf, senha, tipo_perfil, data_criacao, ativo, aceite_lgpd
                FROM usuario
                WHERE email = @Email
                LIMIT 1;
                """;

            using IDbConnection conn = new SqliteConnection(_connectionString);

            var row = await conn.QuerySingleOrDefaultAsync(sql, new { Email = email.Trim().ToLowerInvariant() });

            if (row is null)
                return null;

            return Usuario.FromDatabase(
                id:          (int)row.id,
                nome:        (string)row.nome,
                email:       (string)row.email,
                cpf:         (string)row.cpf,
                senhaHash:   (string)row.senha,
                tipoPerfil:  (string)row.tipo_perfil,
                dataCriacao: Convert.ToDateTime(row.data_criacao),
                ativo:       Convert.ToInt32(row.ativo) == 1,
                aceiteLgpd:  Convert.ToInt32(row.aceite_lgpd) == 1
            );
        }

        public async Task<bool> CadastrarUsuario(Usuario usuario)
        {
            const string sql = @"
                INSERT INTO usuario 
                    (nome, email, cpf, senha, tipo_perfil, data_criacao, ativo, aceite_lgpd)
                VALUES 
                    (@Nome, @Email, @Cpf, @SenhaHash, @TipoPerfil, @DataCriacao, @Ativo, @AceiteLgpd)";

            using IDbConnection conn = new SqliteConnection(_connectionString);
            int rowsAffected = await conn.ExecuteAsync(sql, new {
                usuario.Nome,
                usuario.Email,
                usuario.Cpf,
                usuario.SenhaHash,
                usuario.TipoPerfil,
                DataCriacao = usuario.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                Ativo = usuario.Ativo ? 1 : 0,
                AceiteLgpd = usuario.AceiteLgpd ? 1 : 0
            });

            return rowsAffected > 0;
        }
    }
}