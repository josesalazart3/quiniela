using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IFaseService
    {
        Task<FaseReadDto> CreateFaseAsync(FaseCreateDto dto);
        Task<IEnumerable<FaseReadDto>> GetAllFasesAsync(PaginacionDto paginacion);
        Task<IEnumerable<FaseReadDto>> GetFasesByTorneoAsync(int torneoId);
        Task<IEnumerable<FaseSelectDto>> GetFasesSelectAsync(int torneoId);
        Task<FaseReadDto?> GetFaseByIdAsync(int id);
        Task<FaseReadDto?> UpdateFaseAsync(int id, FaseUpdateDto dto);
        Task<bool> DeleteFaseAsync(int id);
    }
}