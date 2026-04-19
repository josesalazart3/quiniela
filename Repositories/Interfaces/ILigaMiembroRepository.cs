using Quiniela.Models;
using Quiniela.Enums;

namespace Quiniela.Repositories.Interfaces
{
    public interface ILigaMiembroRepository
    {
        Task<LigaMiembro> AddMiembroAsync(LigaMiembro miembro);
        Task<IEnumerable<LigaMiembro>> GetMiembrosByLigaAsync(int ligaId);
        Task<IEnumerable<LigaMiembro>> GetMiembrosPendientesByLigaAsync(int ligaId);
        Task<LigaMiembro?> GetMiembroAsync(int userId, int ligaId);
        Task<LigaMiembro?> UpdateMiembroAsync(LigaMiembro miembro);
        Task<bool> DeleteMiembroAsync(int userId, int ligaId);
        Task<bool> EsMiembroAsync(int userId, int ligaId);
        Task<bool> EsAdminAsync(int userId, int ligaId);
    }
}