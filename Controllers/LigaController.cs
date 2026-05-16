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
    public class LigaController(ILigaService ligaService, CryptoHelper cryptoHelper, ILogger<LigaController> logger) : ControllerBase
    {
        private readonly ILigaService _ligaService = ligaService;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;
        private readonly ILogger<LigaController> _logger = logger;

        private int GetUserId()
        {
            var encrypted = User.FindFirst("Id")?.Value ?? string.Empty;
            var decrypted = _cryptoHelper.Decrypt(encrypted);
            return int.Parse(decrypted);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var ligas = await _ligaService.GetAllLigasAsync(paginacion);
            return Ok(ligas);
        }

        [HttpGet("mis-ligas")]
        public async Task<IActionResult> GetMisLigas([FromQuery] PaginacionDto paginacion)
        {
            var userId = GetUserId();
            var ligas = await _ligaService.GetMisLigasAsync(userId, paginacion);
            return Ok(ligas);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Search([FromQuery] string nombre, [FromQuery] PaginacionDto paginacion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest(new { error = "El nombre de búsqueda es obligatorio" });

            var ligas = await _ligaService.SearchLigasAsync(nombre, paginacion);
            return Ok(ligas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var liga = await _ligaService.GetLigaByIdAsync(id);
            if (liga == null) return NotFound();
            return Ok(liga);
        }

        [HttpGet("{ligaId:int}/miembros")]
        public async Task<IActionResult> GetMiembros(int ligaId)
        {
            var miembros = await _ligaService.GetMiembrosByLigaAsync(ligaId);
            return Ok(miembros);
        }

        [HttpGet("{ligaId:int}/miembros/pendientes")]
        public async Task<IActionResult> GetMiembrosPendientes(int ligaId)
        {
            try
            {
                var adminId = GetUserId();
                var miembros = await _ligaService.GetMiembrosPendientesAsync(ligaId, adminId);
                return Ok(miembros);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        /// <summary>
        /// Al crear la liga el usuario queda automáticamente como admin y miembro aprobado.
        /// Si es de apuestas el precio es obligatorio y mayor a 0.
        /// </summary>
        public async Task<IActionResult> Create([FromBody] LigaCreateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var created = await _ligaService.CreateLigaAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear liga");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        [HttpPost("{ligaId:int}/unirse")]
        /// <summary>
        /// El usuario queda en estado Pendiente hasta que el admin lo apruebe.
        /// </summary>
        public async Task<IActionResult> Unirse(int ligaId, [FromBody] LigaMiembroCreateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var miembro = await _ligaService.UnirseALigaAsync(ligaId, dto, userId);
                return Ok(miembro);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al unirse a liga");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        [HttpPost("{ligaId:int}/aprobar")]
        public async Task<IActionResult> AprobarMiembro(int ligaId, [FromBody] LigaMiembroAprobacionDto dto)
        {
            try
            {
                var adminId = GetUserId();
                var miembro = await _ligaService.AprobarMiembroAsync(ligaId, dto, adminId);
                return Ok(miembro);
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
                _logger.LogError(ex, "Error al aprobar miembro");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] LigaUpdateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var updated = await _ligaService.UpdateLigaAsync(id, dto, userId);
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
                _logger.LogError(ex, "Error al actualizar liga");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        [HttpGet("{ligaId:int}/ranking")]
        public async Task<IActionResult> GetRanking(int ligaId)
        {
            var ranking = await _ligaService.GetRankingLigaAsync(ligaId);
            return Ok(ranking);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                var deleted = await _ligaService.DeleteLigaAsync(id, userId);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar liga");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        [HttpDelete("{ligaId:int}/salir")]
        public async Task<IActionResult> Salir(int ligaId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _ligaService.SalirDeLigaAsync(ligaId, userId);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al salir de liga");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }
    }
}