using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IEstadioRepository
    {
        Task<Estadio> AddEstadioAsync(Estadio estadio);
        Task<IEnumerable<Estadio>> GetAllEstadiosAsync(int page, int pageSize);
        Task<IEnumerable<Estadio>> GetEstadiosSelectAsync();
        Task<Estadio?> GetEstadioByIdAsync(int id);
        Task<Estadio?> UpdateEstadioAsync(Estadio estadio);
        Task<bool> DeleteEstadioAsync(int id);
    }
}