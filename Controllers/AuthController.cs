using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Quiniela.Utils;
using Microsoft.AspNetCore.Authorization;


namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IAuthService authService, CryptoHelper cryptoHelper, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.AuthenticateAsync(request);
            if (response == null)
                return Unauthorized(new { error = "Invalid username or password" });

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return CreatedAtAction(nameof(Register), new { message = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(ex, "Error al registrar usuario");
                return StatusCode(500, new { error = "Ocurrió un error inesperado" });
            }

        }

        [HttpPut("reset/{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassoword(int id, [FromBody] ResetPasswordRequestDto dto)
        {
            var encryptedUserId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(encryptedUserId))
                return Unauthorized();

            var decryptedUserId = _cryptoHelper.Decrypt(encryptedUserId);
            if (!int.TryParse(decryptedUserId, out int userId))
                return Unauthorized();

            var message = await _authService.ResetPasswordAsync(id, dto, userId);

            return Ok(new { message });
        }

    }
}