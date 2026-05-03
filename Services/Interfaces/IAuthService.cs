using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IAuthService
    {
        //Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
        Task<string> RegisterAsync(RegisterRequestDto request);
        Task<string> ResetPasswordAsync(int id, ResetPasswordRequestDto request, int requestingUserId);
        Task<UserProfileDto?> GetProfileAsync(int userId);
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request, string ipOrigen, string userAgent);
        Task LogoutAsync(int userId);

        Task ForgotPasswordAsync(string email);
        Task RecoverPasswordAsync(RecoverPasswordDto dto);

    }
}