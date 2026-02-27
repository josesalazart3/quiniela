using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
        Task<string?> RegisterAsync(RegisterRequestDto request);

        Task<string>  ResetPasswordAsync(int id, ResetPasswordRequestDto request, int userId); //-> userId por si se me anotoja que pueda actualizar algun admin
    }
}