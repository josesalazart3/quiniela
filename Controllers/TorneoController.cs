using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class TorneoController(ITorneoService torneoService, ILogger<TorneoController> logger) : ControllerBase
    {
        private readonly ITorneoService _torneoService = torneoService;
        private readonly ILogger<TorneoController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var torneos = await _torneoService.GetAllTorneosAsync(paginacion);
            return Ok(torneos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var torneo = await _torneoService.GetTorneoByIdAsync(id);
            if (torneo == null) return NotFound();
            return Ok(torneo);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([FromBody] TorneoCreateDto dto)
        {
            try
            {
                var created = await _torneoService.CreateTorneoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear torneo");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] TorneoUpdateDto dto)
        {
            try
            {
                var updated = await _torneoService.UpdateTorneoAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar torneo");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _torneoService.DeleteTorneoAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar torneo");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var torneos = await _torneoService.GetTorneosSelectAsync();
            return Ok(torneos);
        }
    }
}