using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IRankingService
    {
        Task<IEnumerable<RankingGlobalUsuarioReadDto>> GetRankingGlobalUsuariosAsync();
        Task<IEnumerable<RankingGlobalLigaReadDto>> GetRankingGlobalLigasAsync();
        Task<IEnumerable<RankingLigaReadDto>> GetPremiosLigaAsync(int ligaId);
        Task<PremiosGlobalesReadDto> GetPremiosGlobalesAsync();
    }
}