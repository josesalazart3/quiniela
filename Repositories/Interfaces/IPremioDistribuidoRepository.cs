using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IPremioDistribuidoRepository
    {
        Task AddRangeAsync(List<PremioDistribuido> premios);
        Task<IEnumerable<PremioDistribuido>> GetByTorneoAsync(int torneoId);
    }
}