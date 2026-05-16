using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Quiniela.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IAuthService authService, CryptoHelper cryptoHelper) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            var response = await _authService.AuthenticateAsync(request, ip, userAgent);
            if (response == null)
                return Unauthorized(new { error = "Credenciales inválidas" });

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { message = result });
        }

        [HttpPut("reset/{id:int}")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequestDto dto)
        {
            var encryptedUserId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(encryptedUserId))
                return Unauthorized();

            if (!int.TryParse(_cryptoHelper.Decrypt(encryptedUserId), out int userId))
                return Unauthorized();

            var message = await _authService.ResetPasswordAsync(id, dto, userId);
            return Ok(new { message });
        }

        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var encrypted = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(encrypted)) return Unauthorized();

            if (!int.TryParse(_cryptoHelper.Decrypt(encrypted), out int userId))
                return Unauthorized();

            var profile = await _authService.GetProfileAsync(userId);
            if (profile == null) return NotFound();

            return Ok(profile);
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var encrypted = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(encrypted)) return Unauthorized();

            if (!int.TryParse(_cryptoHelper.Decrypt(encrypted), out int userId))
                return Unauthorized();

            await _authService.LogoutAsync(userId);
            return Ok(new { message = "Sesión cerrada correctamente" });
        }

        [HttpPost("contraseña-olvidada")]
        /// <summary>
        /// Siempre retorna 200 aunque el email no exista por seguridad no se revela si el email está registrado.
        /// </summary>
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto.Email);
            // Siempre retorna 200 aunque el email no exista — por seguridad
            return Ok(new { message = "Si el email existe recibirás un correo con instrucciones" });
        }

        [HttpPost("recuperar-contraseña")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDto dto)
        {
            await _authService.RecoverPasswordAsync(dto);
            return Ok(new { message = "Contraseña actualizada correctamente" });
        }
    }
}