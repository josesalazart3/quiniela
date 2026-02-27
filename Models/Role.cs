using System.ComponentModel.DataAnnotations;

namespace Quiniela.Models
{
    public class Role
    {
        public int Id {get; set;}

        [Required]
        [MaxLength(30)]
        public required string Name {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set;}
        public ICollection<User> Users {get; set;} = new List<User>();
    }
}