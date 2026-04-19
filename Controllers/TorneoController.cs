using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class TorneoController(ITorneoService torneoService) : ControllerBase
    {
        private readonly ITorneoService _torneoService = torneoService;

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
            var created = await _torneoService.CreateTorneoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] TorneoUpdateDto dto)
        {
            var updated = await _torneoService.UpdateTorneoAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _torneoService.DeleteTorneoAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var torneos = await _torneoService.GetTorneosSelectAsync();
            return Ok(torneos);
        }
    }
}