namespace Quiniela.Models.DTOs
{
    public class LoginResponseDto
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required RoleDto Role { get; set; }
    }
}