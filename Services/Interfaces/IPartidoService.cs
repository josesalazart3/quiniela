using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IPartidoService
    {
        Task<PartidoReadDto> CreatePartidoAsync(PartidoCreateDto dto);
        Task<IEnumerable<PartidoReadDto>> GetAllPartidosAsync(PaginacionDto paginacion);
        Task<IEnumerable<PartidoReadDto>> GetPartidosByTorneoAsync(int torneoId);
        Task<IEnumerable<PartidoReadDto>> GetPartidosByFaseAsync(int faseId);
        Task<IEnumerable<PartidoReadDto>> GetPartidosByGrupoAsync(int grupoId);
        Task<IEnumerable<PartidoReadDto>> GetPartidosPendientesAsync(int torneoId);
        Task<PartidoReadDto?> GetPartidoByIdAsync(int id);
        Task<PartidoReadDto?> UpdatePartidoAsync(int id, PartidoUpdateDto dto);
        Task<PartidoReadDto?> IngresarResultadoAsync(int id, PartidoResultadoDto dto);
        Task<bool> DeletePartidoAsync(int id);
        Task<PartidoReadDto?> ActualizarMarcadorAsync(int id, PartidoMarcadorDto dto);

    }
}