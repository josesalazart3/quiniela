using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
        Task<string> RegisterAsync(RegisterRequestDto request);
        Task<string> ResetPasswordAsync(int id, ResetPasswordRequestDto request, int requestingUserId);
        Task<UserProfileDto?> GetProfileAsync(int userId);
    }
}