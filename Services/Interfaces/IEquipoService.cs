using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IEquipoService
    {
        Task<EquipoReadDto> CreateEquipoAsync(EquipoCreateDto dto);
        Task<IEnumerable<EquipoReadDto>> GetAllEquiposAsync(PaginacionDto paginacion);
        Task<IEnumerable<EquipoSelectDto>> GetEquiposSelectAsync();
        Task<EquipoReadDto?> GetEquipoByIdAsync(int id);
        Task<EquipoReadDto?> UpdateEquipoAsync(int id, EquipoUpdateDto dto);
        Task<bool> DeleteEquipoAsync(int id);
    }
}