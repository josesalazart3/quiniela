using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        [MinLength(3, ErrorMessage = "Mínimo 3 caracteres")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [MaxLength(100)]
        public required string Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;


        //el role lo definiré en el servicio para que todos los usuarios que se registren sea como usuario normal jsjssj
    }
}