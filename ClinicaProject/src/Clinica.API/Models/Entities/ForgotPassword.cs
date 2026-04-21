

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Data;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

namespace Clinica.API.Models.DTOs.ForgotPassword{
    public class ForgotPassword(string email)
{
    string? emailRecuperacao;

    string verificarEmail= $'SELECT EXISTS(SELECT 1 FROM usuario WHERE email='{emailRecuperação}'
    
}
}