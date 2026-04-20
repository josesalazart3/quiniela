using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Quiniela.Enums;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class UserSessionRepository(AppDbContext context) : IUserSessionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<UserSession> CreateSessionAsync(UserSession session)
        {
            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<UserSession?> GetActiveSessionAsync(int userId)
        {
            return await _context.UserSessions
                .Where(s => s.UserId == userId && s.Estado == EstadoSesion.Activa)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserSession>> GetSessionsByUserAsync(int userId)
        {
            return await _context.UserSessions
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CloseSessionAsync(int sessionId)
        {
            var session = await _context.UserSessions.FindAsync(sessionId);
            if (session == null) return false;

            session.Estado = EstadoSesion.Cerrada;
            session.ClosedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task MarkExpiredSessionsAsync()
        {
            var expiredSessions = await _context.UserSessions
                .Where(s => s.Estado == EstadoSesion.Activa && s.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            foreach (var session in expiredSessions)
            {
                session.Estado = EstadoSesion.Expirada;
                session.ClosedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}