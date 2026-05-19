using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ReporteController(IReporteService reporteService) : ControllerBase
    {
        private readonly IReporteService _reporteService = reporteService;

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _reporteService.GetResumenAsync();
            return Ok(resumen);
        }

        [HttpGet("usuarios/descargar")]
        public async Task<IActionResult> DescargarUsuarios()
        {
            var csv = await _reporteService.ExportUsuariosCsvAsync();
            var fileName = $"usuarios_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
            return File(csv, "text/csv", fileName);
        }

        [HttpGet("ligas/descargar")]
        public async Task<IActionResult> DescargarLigas()
        {
            var csv = await _reporteService.ExportLigasCsvAsync();
            var fileName = $"ligas_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
            return File(csv, "text/csv", fileName);
        }

        [HttpGet("predicciones/descargar/{ligaId:int}")]
        public async Task<IActionResult> DescargarPredicciones(int ligaId)
        {
            var csv = await _reporteService.ExportPrediccionesCsvAsync(ligaId);
            var fileName = $"predicciones_liga_{ligaId}_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
            return File(csv, "text/csv", fileName);
        }

        [HttpGet("premios/descargar/{torneoId:int}")]
        public async Task<IActionResult> DescargarPremios(int torneoId)
        {
            var csv = await _reporteService.ExportPremiosDistribuidosCsvAsync(torneoId);
            var fileName = $"premios_torneo_{torneoId}_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
            return File(csv, "text/csv", fileName);
        }
    }
}