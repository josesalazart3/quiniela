using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailWithRoleAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null) return null;

            existing.Email = user.Email;
            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.UpdatedAt = user.UpdatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> UpdatePasswordAsync(int id, string hashedPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Password = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserByIdWithRoleAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}