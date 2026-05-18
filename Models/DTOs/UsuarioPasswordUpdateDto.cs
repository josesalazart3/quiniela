using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class UsuarioPasswordUpdateDto
    {
        [Required]
        [MinLength(6)]
        public required string NewPassword { get; set; }
    }
}