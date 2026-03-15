using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public required string Password { get; set; }
    }
}