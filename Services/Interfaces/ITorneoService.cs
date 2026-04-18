using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface ITorneoService
    {
        Task<TorneoReadDto> CreateTorneoAsync(TorneoCreateDto dto);
        Task<IEnumerable<TorneoReadDto>> GetAllTorneosAsync(PaginacionDto paginacion);
        Task<TorneoReadDto?> GetTorneoByIdAsync(int id);
        Task<TorneoReadDto?> UpdateTorneoAsync(int id, TorneoUpdateDto dto);
        Task<bool> DeleteTorneoAsync(int id);
        Task<IEnumerable<TorneoSelectDto>> GetTorneosSelectAsync();
    }
}