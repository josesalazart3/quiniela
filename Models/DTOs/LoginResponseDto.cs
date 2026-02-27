
namespace Quiniela.Models.DTOs
{
    public class LoginResponseDto
    {
        public required string Token {get; set;}
        public required string Username {get; set;}
        public required RoleDto Role {get; set;}
    }
}