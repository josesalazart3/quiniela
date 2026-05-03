using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class PasswordResetTokenRepository(AppDbContext context) : IPasswordResetTokenRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<PasswordResetToken> CreateTokenAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Token == token &&
                    !t.Usado &&
                    t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<bool> InvalidateTokenAsync(int tokenId)
        {
            var token = await _context.PasswordResetTokens.FindAsync(tokenId);
            if (token == null) return false;

            token.Usado = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task InvalidateAllUserTokensAsync(int userId)
        {
            var tokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == userId && !t.Usado)
                .ToListAsync();

            foreach (var token in tokens)
                token.Usado = true;

            await _context.SaveChangesAsync();
        }
    }
}