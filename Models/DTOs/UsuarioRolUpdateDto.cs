using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models.DTOs
{
    public class UsuarioRolUpdateDto
    {
        [Required]
        public required int RoleId { get; set; }
    }
}