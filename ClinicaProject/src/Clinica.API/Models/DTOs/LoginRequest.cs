using System.ComponentModel.DataAnnotations;

namespace Clinica.API.Models.DTOs
{
    public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required]               string Senha
    );
}