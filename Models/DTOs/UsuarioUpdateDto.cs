using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class UsuarioUpdateDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }
    }
}