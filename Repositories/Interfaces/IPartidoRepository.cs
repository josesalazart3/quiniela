using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IPartidoRepository
    {
        Task<Partido> AddPartidoAsync(Partido partido);
        Task<IEnumerable<Partido>> GetAllPartidosAsync(int page, int pageSize);
        Task<IEnumerable<Partido>> GetPartidosByTorneoAsync(int torneoId);
        Task<IEnumerable<Partido>> GetPartidosByFaseAsync(int faseId);
        Task<IEnumerable<Partido>> GetPartidosByGrupoAsync(int grupoId);
        Task<IEnumerable<Partido>> GetPartidosPendientesAsync(int torneoId);
        Task<Partido?> GetPartidoByIdAsync(int id);
        Task<Partido?> GetPartidoByIdWithDetailsAsync(int id);
        Task<Partido?> UpdatePartidoAsync(Partido partido);
        Task<Partido?> IngresarResultadoAsync(int id, int golesLocal, int golesVisitante);
        Task<bool> DeletePartidoAsync(int id);
        Task<IEnumerable<Partido>> GetPartidosFinalizadosByGrupoAsync(int grupoId);
    }
}