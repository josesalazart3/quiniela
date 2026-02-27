using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models
{
    public class User
    {
        public int Id {get; set;}

        [Required]
        [MaxLength(50)]
        public required string Username {get; set;}

        [Required]
        [MaxLength(255)]
        public required string Password {get; set;}

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string Email {get; set;}

        [MaxLength(50)]
        public string FirstName {get; set;} = string.Empty;

        [MaxLength(50)]
        public string LastName {get; set;} = string.Empty;
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set;}

        [Required]
        public required int RoleId {get; set;}
        public Role? Role {get; set;}
    }
}