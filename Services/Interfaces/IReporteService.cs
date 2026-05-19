using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IReporteService
    {
        Task<ReporteResumenDto> GetResumenAsync();
        Task<byte[]> ExportUsuariosCsvAsync();
        Task<byte[]> ExportLigasCsvAsync();
        Task<byte[]> ExportPrediccionesCsvAsync(int ligaId);
        Task<byte[]> ExportPremiosDistribuidosCsvAsync(int torneoId);
    }
}