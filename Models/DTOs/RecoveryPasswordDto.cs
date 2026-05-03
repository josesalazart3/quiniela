using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class RecoverPasswordDto
    {
        [Required(ErrorMessage = "El token es obligatorio")]
        public required string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public required string NewPassword { get; set; }
    }
}