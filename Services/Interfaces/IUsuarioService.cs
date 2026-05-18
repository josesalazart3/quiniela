using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UserProfileDto>> GetAllAsync(PaginacionDto paginacion);
        Task<UserProfileDto?> GetByIdAsync(int id);
        Task<UserProfileDto> CreateAsync(UsuarioCreateDto dto);
        Task<UserProfileDto?> UpdateAsync(int id, UsuarioUpdateDto dto);
        Task<UserProfileDto?> UpdateRoleAsync(int id, UsuarioRolUpdateDto dto);
        Task<bool> UpdatePasswordAsync(int id, UsuarioPasswordUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}