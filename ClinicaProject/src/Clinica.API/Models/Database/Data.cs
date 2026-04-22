using System.Data;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Clinica.API.Models.Entities;
using Microsoft.Data.Sqlite;

public class Data(){
static string connectionString= "Server=localhost;Database=AgendaiFisio";
static SqliteConnection bancoConexao=new SqliteConnection(connectionString);

public static void AbrirConexao()

{
    bancoConexao.Open(); 
}


public static bool VerificarEmail(string email)
{
    //Comando em QUERY para verificar a existência do E-mail de verificação
    string comando= $"SELECT EXISTS(SELECT 1 FROM usuario WHERE email= '{email}'";
    //Convertendo a string em comando
    SqliteCommand comandoConsultarEmail=new SqliteCommand(comando);
    
    //Retorna True ou False (Fazendo conversão pois o método retorna 1 ou 0)
    //Necessário que a conexão esteja aberta
    return Convert.ToBoolean(comandoConsultarEmail.ExecuteReader());

}


public static void FecharConexao(){
    bancoConexao.Close();
}
}