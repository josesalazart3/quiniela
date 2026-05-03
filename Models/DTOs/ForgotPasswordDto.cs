using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public required string Email { get; set; }
    }
}