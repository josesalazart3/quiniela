using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;
using Quiniela.Utils;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PrediccionController(IPrediccionService prediccionService, CryptoHelper cryptoHelper, ILogger<PrediccionController> logger) : ControllerBase
    {
        private readonly IPrediccionService _prediccionService = prediccionService;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;
        private readonly ILogger<PrediccionController> _logger = logger;

        private int GetUserId()
        {
            var encrypted = User.FindFirst("Id")?.Value ?? string.Empty;
            var decrypted = _cryptoHelper.Decrypt(encrypted);
            return int.Parse(decrypted);
        }

        [HttpGet("liga/{ligaId:int}")]
        public async Task<IActionResult> GetByLiga(int ligaId, [FromQuery] PaginacionDto paginacion)
        {
            var predicciones = await _prediccionService.GetPrediccionesByLigaAsync(ligaId, paginacion);
            return Ok(predicciones);
        }

        [HttpGet("mis-predicciones/{ligaId:int}")]
        public async Task<IActionResult> GetMisPredicciones(int ligaId, [FromQuery] PaginacionDto paginacion)
        {
            var userId = GetUserId();
            var predicciones = await _prediccionService.GetMisPrediccionesAsync(userId, ligaId, paginacion);
            return Ok(predicciones);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prediccion = await _prediccionService.GetPrediccionByIdAsync(id);
            if (prediccion == null) return NotFound();
            return Ok(prediccion);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrediccionCreateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var created = await _prediccionService.CreatePrediccionAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear predicción");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PrediccionUpdateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var updated = await _prediccionService.UpdatePrediccionAsync(id, dto, userId);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar predicción");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }
    }
}