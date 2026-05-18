using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);//
        Task<User?> GetByEmailWithRoleAsync(string email);//
        Task<User> AddUserAsync(User user);//
        Task<User?> GetUserByIdAsync(int id);//
        //Task<User?> UpdateUserAsync(User user);
        Task<bool> UpdatePasswordAsync(int id, string newPassword);

        Task<User?> GetUserByIdWithRoleAsync(int id);//
        Task<bool> DeleteUserAsync(int id);//
        Task<IEnumerable<User>> GetAllUsersAsync(int page, int pageSize);//
        Task<bool> UpdateUserAsync(int id, string email, string firstName, string lastName);//
        Task<bool> UpdateUserRoleAsync(int id, int roleId);//

    }
}