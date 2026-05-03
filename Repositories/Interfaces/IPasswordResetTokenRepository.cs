using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken> CreateTokenAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetValidTokenAsync(string token);
        Task<bool> InvalidateTokenAsync(int tokenId);
        Task InvalidateAllUserTokensAsync(int userId);
    }
}