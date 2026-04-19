using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IPrediccionRepository
    {
        Task<Prediccion> AddPrediccionAsync(Prediccion prediccion);
        Task<IEnumerable<Prediccion>> GetPrediccionesByLigaAsync(int ligaId, int page, int pageSize);
        Task<IEnumerable<Prediccion>> GetPrediccionesByUserYLigaAsync(int userId, int ligaId, int page, int pageSize);
        Task<IEnumerable<Prediccion>> GetPrediccionesByPartidoAsync(int partidoId);
        Task<Prediccion?> GetPrediccionByIdAsync(int id);
        Task<Prediccion?> GetPrediccionByUserLigaPartidoAsync(int userId, int ligaId, int partidoId);
        Task<Prediccion?> UpdatePrediccionAsync(Prediccion prediccion);
        Task ActualizarPuntosPrediccionesAsync(int partidoId, int golesLocal, int golesVisitante);
        Task<bool> DeletePrediccionAsync(int id);
    }
}
