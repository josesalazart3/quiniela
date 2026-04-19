using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class FaseController(IFaseService faseService) : ControllerBase
    {
        private readonly IFaseService _faseService = faseService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var fases = await _faseService.GetAllFasesAsync(paginacion);
            return Ok(fases);
        }

        [HttpGet("torneo/{torneoId:int}")]
        public async Task<IActionResult> GetByTorneo(int torneoId)
        {
            var fases = await _faseService.GetFasesByTorneoAsync(torneoId);
            return Ok(fases);
        }

        [HttpGet("select/{torneoId:int}")]
        public async Task<IActionResult> GetSelect(int torneoId)
        {
            var fases = await _faseService.GetFasesSelectAsync(torneoId);
            return Ok(fases);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var fase = await _faseService.GetFaseByIdAsync(id);
            if (fase == null) return NotFound();
            return Ok(fase);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([FromBody] FaseCreateDto dto)
        {
            var created = await _faseService.CreateFaseAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] FaseUpdateDto dto)
        {
            var updated = await _faseService.UpdateFaseAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _faseService.DeleteFaseAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
