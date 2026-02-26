namespace Quiniela.Models
{
    public class User
    {
        public int Id {get; set;}
        public required string Username {get; set;}
        public required string Password {get; set;}
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set;}
        
    }
}