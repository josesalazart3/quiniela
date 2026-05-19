using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class UsuarioService(
        IUserRepository userRepository,
        IRoleRepository roleRepository) : IUsuarioService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<IEnumerable<UserProfileDto>> GetAllAsync(PaginacionDto paginacion)
        {
            var users = await _userRepository.GetAllUsersAsync(paginacion.Page, paginacion.PageSize);
            return users.Select(MapToDto);
        }

        public async Task<UserProfileDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdWithRoleAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserProfileDto> CreateAsync(UsuarioCreateDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("El email ya está en uso");

            var role = await _roleRepository.GetRoleByIdAsync(dto.RoleId);
            if (role == null)
                throw new InvalidOperationException("El rol especificado no existe");

            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);

            var created = await _userRepository.GetUserByIdWithRoleAsync(user.Id);
            return MapToDto(created!);
        }

        public async Task<UserProfileDto?> UpdateAsync(int id, UsuarioUpdateDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("El email ya está en uso");

            var updated = await _userRepository.UpdateUserAsync(id, dto.Email, dto.FirstName, dto.LastName);
            if (!updated) return null;

            var user = await _userRepository.GetUserByIdWithRoleAsync(id);
            return MapToDto(user!);
        }

        public async Task<UserProfileDto?> UpdateRoleAsync(int id, UsuarioRolUpdateDto dto)
        {
            var role = await _roleRepository.GetRoleByIdAsync(dto.RoleId);
            if (role == null)
                throw new InvalidOperationException("El rol especificado no existe");

            var updated = await _userRepository.UpdateUserRoleAsync(id, dto.RoleId);
            if (!updated) return null;

            var user = await _userRepository.GetUserByIdWithRoleAsync(id);
            return MapToDto(user!);
        }

        public async Task<bool> UpdatePasswordAsync(int id, UsuarioPasswordUpdateDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return false;

            return await _userRepository.UpdatePasswordAsync(id, BCrypt.Net.BCrypt.HashPassword(dto.NewPassword));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        private static UserProfileDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Role = user.Role?.Name ?? string.Empty
        };

        public async Task<IEnumerable<RoleSelectDto>> GetRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return roles.Select(r => new RoleSelectDto
            {
                Id = r.Id,
                Name = r.Name
            });
        }
    }
}