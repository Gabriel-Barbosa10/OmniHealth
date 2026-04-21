using System.Data;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Clinica.API.Models.Entities;
using Microsoft.Data.Sqlite;

public class UsuarioContext : DbConnect
{
public DbSet<UsuarioContext> {get:set}}

static string connectionString= "Server=localhost;Database=AgendaiFisio";
static var bancoConexao=new SqliteConnection(connectionString);

public static void AbrirConexao()

{
    bancoConexao.Open(); 
}


public static bool VerificarEmail(string email)
{

    email=email.Trim();

}



public static void FecharConexao(){
    bancoConexao.Close();
}