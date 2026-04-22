

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Data;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Clinica.API.Models;

namespace Clinica.API.Models.DTOs.ForgotPassword{
    
public class ForgotPassword()
    {
        static void RecuperarSenha(string email)
        {
            email.Trim();

            if(Data.VerificarEmail(email) is false)
                Console.WriteLine("Não há nenhuma conta cadastrada com este e-mail");


            else
            {
                //Geração do Token de recuperação


                
            }

        }



    }
}