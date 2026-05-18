using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class UsuarioCreateDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }

        [Required]
        public required int RoleId { get; set; }
    }
}