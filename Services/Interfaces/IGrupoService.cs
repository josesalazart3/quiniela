using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IGrupoService
    {
        Task<GrupoReadDto> CreateGrupoAsync(GrupoCreateDto dto);
        Task<IEnumerable<GrupoReadDto>> GetAllGruposAsync(PaginacionDto paginacion);
        Task<IEnumerable<GrupoReadDto>> GetGruposByTorneoAsync(int torneoId);
        Task<IEnumerable<GrupoSelectDto>> GetGruposSelectAsync(int torneoId);
        Task<GrupoReadDto?> GetGrupoByIdAsync(int id);
        Task<GrupoReadDto?> UpdateGrupoAsync(int id, GrupoUpdateDto dto);
        Task<bool> DeleteGrupoAsync(int id);
        Task<GrupoReadDto> AsignarEquipoAGrupoAsync(int grupoId, int equipoId);
        Task<bool> RemoverEquipoDeGrupoAsync(int grupoId, int equipoId);
        Task<GrupoReadDto> AsignarVariosEquiposAGrupoAsync(int grupoId, GrupoEquipoAsignarVariosDto dto);
        Task<IEnumerable<ClasificacionGrupoReadDto>> GetClasificacionByGrupoAsync(int grupoId);

    }
}