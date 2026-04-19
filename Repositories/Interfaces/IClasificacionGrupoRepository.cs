using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IClasificacionGrupoRepository
    {
        Task<ClasificacionGrupo> AddClasificacionAsync(ClasificacionGrupo clasificacion);
        Task<IEnumerable<ClasificacionGrupo>> GetClasificacionByGrupoAsync(int grupoId);
        Task<ClasificacionGrupo?> GetClasificacionByGrupoYEquipoAsync(int grupoId, int equipoId);
        Task<ClasificacionGrupo?> UpdateClasificacionAsync(ClasificacionGrupo clasificacion);
        Task<bool> DeleteClasificacionAsync(int grupoId, int equipoId);
    }
}