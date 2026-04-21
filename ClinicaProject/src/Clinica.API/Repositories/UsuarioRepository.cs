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
                SELECT id, nome, email, cpf, senha, tipo_perfil, aceite_lgpd
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
                senha:       (string)row.senha,
                tipoPerfil:  (string)row.tipo_perfil,
                aceiteLgpd:  (int)row.aceite_lgpd
            );
        }
    }
}