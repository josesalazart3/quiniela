using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface ITorneoRepository
    {
        Task<Torneo> AddTorneoAsync(Torneo torneo);
        Task<IEnumerable<Torneo>> GetAllTorneosAsync(int page, int pageSize);
        Task<Torneo?> GetTorneoByIdAsync(int id);
        Task<Torneo?> GetTorneoByIdWithDetailsAsync(int id);
        Task<Torneo?> UpdateTorneoAsync(Torneo torneo);
        Task<bool> DeleteTorneoAsync(int id);
        Task<IEnumerable<Torneo>> GetTorneosSelectAsync();

    }
}