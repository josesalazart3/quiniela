using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        [MinLength(3, ErrorMessage = "Mínimo 3 caracteres")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public required string Password { get; set; }
    }
}