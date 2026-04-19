using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IEstadioService
    {
        Task<EstadioReadDto> CreateEstadioAsync(EstadioCreateDto dto);
        Task<IEnumerable<EstadioReadDto>> GetAllEstadiosAsync(PaginacionDto paginacion);
        Task<IEnumerable<EstadioSelectDto>> GetEstadiosSelectAsync();
        Task<EstadioReadDto?> GetEstadioByIdAsync(int id);
        Task<EstadioReadDto?> UpdateEstadioAsync(int id, EstadioUpdateDto dto);
        Task<bool> DeleteEstadioAsync(int id);
    }
}