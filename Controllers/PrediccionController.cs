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
    public class PrediccionController(IPrediccionService prediccionService, CryptoHelper cryptoHelper) : ControllerBase
    {
        private readonly IPrediccionService _prediccionService = prediccionService;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;

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

        /// <summary>
        /// Verifica si el usuario ya tiene una predicción para un partido en una liga específica.
        /// </summary>
        [HttpGet("verificar")]
        public async Task<IActionResult> Verificar([FromQuery] int partidoId, [FromQuery] int ligaId)
        {
            var userId = GetUserId();
            var prediccion = await _prediccionService.GetPrediccionByUserLigaPartidoAsync(userId, ligaId, partidoId);
            return Ok(new { existe = prediccion != null, prediccion });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prediccion = await _prediccionService.GetPrediccionByIdAsync(id);
            if (prediccion == null) return NotFound();
            return Ok(prediccion);
        }

        /// <summary>
        /// Solo válido si faltan más de 15 minutos para el inicio del partido.
        /// El usuario debe ser miembro aprobado de la liga.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrediccionCreateDto dto)
        {
            var userId = GetUserId();
            var created = await _prediccionService.CreatePrediccionAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Solo válido si faltan más de 15 minutos para el inicio del partido.
        /// Solo el dueño de la predicción puede modificarla.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PrediccionUpdateDto dto)
        {
            var userId = GetUserId();
            var updated = await _prediccionService.UpdatePrediccionAsync(id, dto, userId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
    }
}