using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> AddRoleAsync(Role role);
        Task<Role?> GetRoleByNameAsync(string name);
        Task<Role?> GetByIdAsync(int id);
        //Task<Role?> UpdateRoleAsync(Role role);
    }
}