using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class EstadioController(IEstadioService estadioService) : ControllerBase
    {
        private readonly IEstadioService _estadioService = estadioService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var estadios = await _estadioService.GetAllEstadiosAsync(paginacion);
            return Ok(estadios);
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var estadios = await _estadioService.GetEstadiosSelectAsync();
            return Ok(estadios);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var estadio = await _estadioService.GetEstadioByIdAsync(id);
            if (estadio == null) return NotFound();
            return Ok(estadio);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([FromBody] EstadioCreateDto dto)
        {
            var created = await _estadioService.CreateEstadioAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] EstadioUpdateDto dto)
        {
            var updated = await _estadioService.UpdateEstadioAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _estadioService.DeleteEstadioAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
