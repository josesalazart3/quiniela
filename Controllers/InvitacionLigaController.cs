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
    public class InvitacionLigaController(
        IInvitacionLigaService invitacionService,
        CryptoHelper cryptoHelper,
        ILogger<InvitacionLigaController> logger) : ControllerBase
    {
        private readonly IInvitacionLigaService _invitacionService = invitacionService;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;
        private readonly ILogger<InvitacionLigaController> _logger = logger;

        private int GetUserId()
        {
            var encrypted = User.FindFirst("Id")?.Value ?? string.Empty;
            var decrypted = _cryptoHelper.Decrypt(encrypted);
            return int.Parse(decrypted);
        }

        // Admin envía invitación
        [HttpPost("liga/{ligaId:int}")]
        public async Task<IActionResult> Enviar(int ligaId, [FromBody] InvitacionCreateDto dto)
        {
            try
            {
                var adminId = GetUserId();
                var invitacion = await _invitacionService.EnviarInvitacionAsync(ligaId, dto, adminId);
                return Ok(invitacion);
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
                _logger.LogError(ex, "Error al enviar invitación");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        // Admin ve las invitaciones de su liga
        [HttpGet("liga/{ligaId:int}")]
        public async Task<IActionResult> GetByLiga(int ligaId)
        {
            try
            {
                var adminId = GetUserId();
                var invitaciones = await _invitacionService.GetInvitacionesByLigaAsync(ligaId, adminId);
                return Ok(invitaciones);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener invitaciones");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        // El invitado responde usando el token del link
        // Este endpoint es público porque el invitado puede no estar registrado
        [HttpPost("responder")]
        [AllowAnonymous]
        public async Task<IActionResult> Responder([FromBody] InvitacionResponderDto dto)
        {
            try
            {
                // Si está logueado pasamos su userId, si no null
                int? userId = null;
                var encrypted = User.FindFirst("Id")?.Value;
                if (!string.IsNullOrEmpty(encrypted))
                {
                    var decrypted = _cryptoHelper.Decrypt(encrypted);
                    if (int.TryParse(decrypted, out int parsedId))
                        userId = parsedId;
                }

                var invitacion = await _invitacionService.ResponderInvitacionAsync(dto, userId);
                return Ok(invitacion);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al responder invitación");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }

        // Admin cancela una invitación
        [HttpDelete("{invitacionId:int}")]
        public async Task<IActionResult> Cancelar(int invitacionId)
        {
            try
            {
                var adminId = GetUserId();
                var cancelada = await _invitacionService.CancelarInvitacionAsync(invitacionId, adminId);
                if (!cancelada) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar invitación");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }
        }
    }
}