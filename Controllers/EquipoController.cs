using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class EquipoController(IEquipoService equipoService) : ControllerBase
    {
        private readonly IEquipoService _equipoService = equipoService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var equipos = await _equipoService.GetAllEquiposAsync(paginacion);
            return Ok(equipos);
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var equipos = await _equipoService.GetEquiposSelectAsync();
            return Ok(equipos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var equipo = await _equipoService.GetEquipoByIdAsync(id);
            if (equipo == null) return NotFound();
            return Ok(equipo);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([FromBody] EquipoCreateDto dto)
        {
            try
            {
                var created = await _equipoService.CreateEquipoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { error = realError });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipoUpdateDto dto)
        {
            var updated = await _equipoService.UpdateEquipoAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _equipoService.DeleteEquipoAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
