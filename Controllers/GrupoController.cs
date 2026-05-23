using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class GrupoController(IGrupoService grupoService) : ControllerBase
    {
        private readonly IGrupoService _grupoService = grupoService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var grupos = await _grupoService.GetAllGruposAsync(paginacion);
            return Ok(grupos);
        }

        [HttpGet("torneo/{torneoId:int}")]
        public async Task<IActionResult> GetByTorneo(int torneoId)
        {
            var grupos = await _grupoService.GetGruposByTorneoAsync(torneoId);
            return Ok(grupos);
        }

        [HttpGet("select/{torneoId:int}")]
        public async Task<IActionResult> GetSelect(int torneoId)
        {
            var grupos = await _grupoService.GetGruposSelectAsync(torneoId);
            return Ok(grupos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var grupo = await _grupoService.GetGrupoByIdAsync(id);
            if (grupo == null) return NotFound();
            return Ok(grupo);
        }

        [HttpGet("{grupoId:int}/clasificacion")]
        public async Task<IActionResult> GetClasificacion(int grupoId)
        {
            var clasificacion = await _grupoService.GetClasificacionByGrupoAsync(grupoId);
            return Ok(clasificacion);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([FromBody] GrupoCreateDto dto)
        {
            try
            {
                var created = await _grupoService.CreateGrupoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("{grupoId:int}/equipos/{equipoId:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AsignarEquipo(int grupoId, int equipoId)
        {
            var grupo = await _grupoService.AsignarEquipoAGrupoAsync(grupoId, equipoId);
            return Ok(grupo);
        }

        [HttpPost("{grupoId:int}/equipos")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AsignarVariosEquipos(int grupoId, [FromBody] GrupoEquipoAsignarVariosDto dto)
        {
            var grupo = await _grupoService.AsignarVariosEquiposAGrupoAsync(grupoId, dto);
            return Ok(grupo);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] GrupoUpdateDto dto)
        {
            var updated = await _grupoService.UpdateGrupoAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _grupoService.DeleteGrupoAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpDelete("{grupoId:int}/equipos/{equipoId:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RemoverEquipo(int grupoId, int equipoId)
        {
            var removido = await _grupoService.RemoverEquipoDeGrupoAsync(grupoId, equipoId);
            if (!removido) return NotFound();
            return NoContent();
        }

        [HttpGet("torneo/{torneoId:int}/mejores-terceros")]
        public async Task<IActionResult> GetMejoresTerceros(int torneoId)
        {
            var terceros = await _grupoService.GetMejoresTercerosAsync(torneoId);
            return Ok(terceros);
        }
    }
}
