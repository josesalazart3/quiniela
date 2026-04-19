using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface ILigaService
    {
        Task<LigaReadDto> CreateLigaAsync(LigaCreateDto dto, int userId);
        Task<IEnumerable<LigaReadDto>> GetAllLigasAsync(PaginacionDto paginacion);
        Task<IEnumerable<LigaReadDto>> GetMisLigasAsync(int userId, PaginacionDto paginacion);
        Task<IEnumerable<LigaReadDto>> SearchLigasAsync(string nombre, PaginacionDto paginacion);
        Task<LigaReadDto?> GetLigaByIdAsync(int id);
        Task<LigaReadDto?> UpdateLigaAsync(int id, LigaUpdateDto dto, int userId);
        Task<bool> DeleteLigaAsync(int id, int userId);
        Task<LigaMiembroReadDto> UnirseALigaAsync(int ligaId, LigaMiembroCreateDto dto, int userId);
        Task<LigaMiembroReadDto> AprobarMiembroAsync(int ligaId, LigaMiembroAprobacionDto dto, int adminId);
        Task<IEnumerable<LigaMiembroReadDto>> GetMiembrosByLigaAsync(int ligaId);
        Task<IEnumerable<LigaMiembroReadDto>> GetMiembrosPendientesAsync(int ligaId, int adminId);
        Task<bool> SalirDeLigaAsync(int ligaId, int userId);
        Task<IEnumerable<RankingLigaReadDto>> GetRankingLigaAsync(int ligaId);

    }
}