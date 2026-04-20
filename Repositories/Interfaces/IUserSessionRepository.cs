using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IUserSessionRepository
    {
        Task<UserSession> CreateSessionAsync(UserSession session);
        Task<UserSession?> GetActiveSessionAsync(int userId);
        Task<IEnumerable<UserSession>> GetSessionsByUserAsync(int userId);
        Task<bool> CloseSessionAsync(int sessionId);
        Task MarkExpiredSessionsAsync();
    }
}