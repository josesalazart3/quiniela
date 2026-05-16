using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class RankingController(IRankingService rankingService, ILogger<RankingController> logger) : ControllerBase
    {
        private readonly IRankingService _rankingService = rankingService;
        private readonly ILogger<RankingController> _logger = logger;

        // Ranking global individual en tiempo real
        [HttpGet("global/usuarios")]
        public async Task<IActionResult> GetRankingGlobalUsuarios()
        {
            var ranking = await _rankingService.GetRankingGlobalUsuariosAsync();
            return Ok(ranking);
        }

        // Ranking global de ligas en tiempo real
        [HttpGet("global/ligas")]
        public async Task<IActionResult> GetRankingGlobalLigas()
        {
            var ranking = await _rankingService.GetRankingGlobalLigasAsync();
            return Ok(ranking);
        }

        // Premios estimados de una liga específica en tiempo real
        [HttpGet("premios/liga/{ligaId:int}")]
        public async Task<IActionResult> GetPremiosLiga(int ligaId)
        {
            var premios = await _rankingService.GetPremiosLigaAsync(ligaId);
            return Ok(premios);
        }

        // Premios globales estimados en tiempo real
        // Solo SystemAdmin puede ver los premios globales
        [HttpGet("premios/globales")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetPremiosGlobales()
        {
            try
            {
                var premios = await _rankingService.GetPremiosGlobalesAsync();
                return Ok(premios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular premios globales");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        /// <summary>
        /// Cierra el torneo, calcula y registra todos los premios definitivos.
        /// Solo puede ejecutarse una vez — el torneo queda marcado como Finalizado.
        /// </summary>
        [HttpPost("premios/globales/cerrar/{torneoId:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CerrarTorneo(int torneoId)
        {
            var premios = await _rankingService.CerrarTorneoYDistribuirPremiosAsync(torneoId);
            return Ok(premios);
        }

        [HttpGet("premios/globales/historial/{torneoId:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetHistorialPremios(int torneoId)
        {
            var premios = await _rankingService.GetPremiosDistribuidosAsync(torneoId);
            return Ok(premios);
        }
    }
}