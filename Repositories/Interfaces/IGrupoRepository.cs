using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IGrupoRepository
    {
        Task<Grupo> AddGrupoAsync(Grupo grupo);
        Task<IEnumerable<Grupo>> GetAllGruposAsync(int page, int pageSize);
        Task<IEnumerable<Grupo>> GetGruposByTorneoAsync(int torneoId);
        Task<IEnumerable<Grupo>> GetGruposSelectAsync(int torneoId);
        Task<Grupo?> GetGrupoByIdAsync(int id);
        Task<Grupo?> GetGrupoByIdWithDetailsAsync(int id);
        Task<Grupo?> UpdateGrupoAsync(Grupo grupo);
        Task<bool> DeleteGrupoAsync(int id);
        Task<bool> EquipoYaEnGrupoAsync(int grupoId, int equipoId);
        Task AddEquipoAGrupoAsync(GrupoEquipo grupoEquipo);
        Task<bool> RemoveEquipoDeGrupoAsync(int grupoId, int equipoId);
        Task AsignarVariosEquiposAsync(List<GrupoEquipo> grupoEquipos);
        Task<IEnumerable<(Grupo Grupo, ClasificacionGrupo Tercero)>> GetTercerosGruposAsync(int torneoId);


    }
}