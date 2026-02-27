using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByUsernameWithRoleAsync(string username);
        Task<User> AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> UpdateUserAsync(User user);
    }
}