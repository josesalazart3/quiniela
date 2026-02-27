using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class ResetPasswordRequestDto
    {
        //public required int UserId {get; set;} lo envio en la ruta
        [Required(ErrorMessage = "La nueva password es obligatoria")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public required string NewPassword {get; set;}
    }
}