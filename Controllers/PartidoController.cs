using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PartidoController(IPartidoService partidoService) : ControllerBase
    {
        private readonly IPartidoService _partidoService = partidoService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var partidos = await _partidoService.GetAllPartidosAsync(paginacion);
            return Ok(partidos);
        }

        [HttpGet("torneo/{torneoId:int}")]
        public async Task<IActionResult> GetByTorneo(int torneoId)
        {
            var partidos = await _partidoService.GetPartidosByTorneoAsync(torneoId);
            return Ok(partidos);
        }

        [HttpGet("fase/{faseId:int}")]
        public async Task<IActionResult> GetByFase(int faseId)
        {
            var partidos = await _partidoService.GetPartidosByFaseAsync(faseId);
            return Ok(partidos);
        }

        [HttpGet("grupo/{grupoId:int}")]
        public async Task<IActionResult> GetByGrupo(int grupoId)
        {
            var partidos = await _partidoService.GetPartidosByGrupoAsync(grupoId);
            return Ok(partidos);
        }

        [HttpGet("pendientes/{torneoId:int}")]
        public async Task<IActionResult> GetPendientes(int torneoId)
        {
            var partidos = await _partidoService.GetPartidosPendientesAsync(torneoId);
            return Ok(partidos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var partido = await _partidoService.GetPartidoByIdAsync(id);
            if (partido == null) return NotFound();
            return Ok(partido);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([FromBody] PartidoCreateDto dto)
        {
            var created = await _partidoService.CreatePartidoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] PartidoUpdateDto dto)
        {
            var updated = await _partidoService.UpdatePartidoAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpPut("{id:int}/resultado")]
        [Authorize(Roles = "Administrador")]
        /// <summary>
        /// crea automáticamente actualización de ClasificacionGrupo, cálculo de puntos
        /// en Predicciones, actualización de LigaMiembro.Puntos, bracket eliminatorio
        /// y notificaciones SignalR a todos los clientes conectados.
        /// </summary>
        public async Task<IActionResult> IngresarResultado(int id, [FromBody] PartidoResultadoDto dto)
        {
            var partido = await _partidoService.IngresarResultadoAsync(id, dto);
            if (partido == null) return NotFound();
            return Ok(partido);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _partidoService.DeletePartidoAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
