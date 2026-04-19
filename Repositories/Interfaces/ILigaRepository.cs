using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface ILigaRepository
    {
        Task<Liga> AddLigaAsync(Liga liga);
        Task<IEnumerable<Liga>> GetAllLigasAsync(int page, int pageSize);
        Task<IEnumerable<Liga>> GetLigasByUserAsync(int userId, int page, int pageSize);
        Task<IEnumerable<Liga>> SearchLigasByNombreAsync(string nombre, int page, int pageSize);
        Task<Liga?> GetLigaByIdAsync(int id);
        Task<Liga?> GetLigaByIdWithDetailsAsync(int id);
        Task<Liga?> UpdateLigaAsync(Liga liga);
        Task<bool> DeleteLigaAsync(int id);
    }
}