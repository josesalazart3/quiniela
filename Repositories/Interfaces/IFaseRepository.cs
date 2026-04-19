using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IFaseRepository
    {
        Task<Fase> AddFaseAsync(Fase fase);
        Task<IEnumerable<Fase>> GetAllFasesAsync(int page, int pageSize);
        Task<IEnumerable<Fase>> GetFasesByTorneoAsync(int torneoId);
        Task<IEnumerable<Fase>> GetFasesSelectAsync(int torneoId);
        Task<Fase?> GetFaseByIdAsync(int id);
        Task<Fase?> UpdateFaseAsync(Fase fase);
        Task<bool> DeleteFaseAsync(int id);
    }
}