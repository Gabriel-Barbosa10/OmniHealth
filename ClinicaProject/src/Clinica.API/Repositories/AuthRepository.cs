using MySql.Data.MySqlClient;
using Clinica.API.Models.Entities;

namespace Clinica.API.Repositories
{
    /// <summary>
    /// Responsável exclusivamente pelo acesso ao banco de dados
    /// para operações de autenticação e cadastro de usuários.
    /// </summary>
    public class AuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Insere um novo usuário na tabela `usuario`.
        /// Campos obrigatórios: nome, email, senha_hash, tipo_perfil, data_criacao.
        /// </summary>
        /// <returns>True se o INSERT foi bem-sucedido.</returns>
        public bool CadastrarUsuario(Usuario usuario)
        {
            const string sql = @"
                INSERT INTO usuario 
                    (nome, email, senha_hash, tipo_perfil, data_criacao, ativo)
                VALUES 
                    (@nome, @email, @senha_hash, @tipo_perfil, @data_criacao, @ativo)";

            using var conexao = new MySqlConnection(_connectionString);
            conexao.Open();

            using var cmd = new MySqlCommand(sql, conexao);
            cmd.Parameters.AddWithValue("@nome",        usuario.Nome);
            cmd.Parameters.AddWithValue("@email",       usuario.Email);
            cmd.Parameters.AddWithValue("@senha_hash",  usuario.SenhaHash);
            cmd.Parameters.AddWithValue("@tipo_perfil", usuario.TipoPerfil);
            cmd.Parameters.AddWithValue("@data_criacao", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@ativo",       true);

            int linhasAfetadas = cmd.ExecuteNonQuery();
            return linhasAfetadas > 0;
        }

        /// <summary>
        /// Busca um usuário ativo pelo e-mail.
        /// Retorna null se não encontrado ou se o usuário estiver inativo.
        /// </summary>
        public Usuario? BuscarPorEmail(string email)
        {
            const string sql = @"
                SELECT id, nome, email, senha_hash, tipo_perfil, data_criacao, ativo
                FROM usuario
                WHERE email = @email
                  AND ativo = TRUE";

            using var conexao = new MySqlConnection(_connectionString);
            conexao.Open();

            using var cmd = new MySqlCommand(sql, conexao);
            cmd.Parameters.AddWithValue("@email", email.Trim().ToLowerInvariant());

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return Usuario.FromDatabase(
                id:          reader.GetInt32("id"),
                nome:        reader.GetString("nome"),
                email:       reader.GetString("email"),
                cpf:         string.Empty,                  
                senhaHash:   reader.GetString("senha_hash"),
                tipoPerfil:  reader.GetString("tipo_perfil"),
                dataCriacao: reader.GetDateTime("data_criacao"),
                ativo:       reader.GetBoolean("ativo")
            );
        }
    }
}