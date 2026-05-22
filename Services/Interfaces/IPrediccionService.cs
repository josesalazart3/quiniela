using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IPrediccionService
    {
        Task<PrediccionReadDto> CreatePrediccionAsync(PrediccionCreateDto dto, int userId);
        Task<IEnumerable<PrediccionReadDto>> GetPrediccionesByLigaAsync(int ligaId, PaginacionDto paginacion);
        Task<IEnumerable<PrediccionReadDto>> GetMisPrediccionesAsync(int userId, int ligaId, PaginacionDto paginacion);
        Task<PrediccionReadDto?> GetPrediccionByIdAsync(int id);
        Task<PrediccionReadDto?> UpdatePrediccionAsync(int id, PrediccionUpdateDto dto, int userId);
        Task<PrediccionReadDto?> GetPrediccionByUserLigaPartidoAsync(int userId, int ligaId, int partidoId);

    }
}