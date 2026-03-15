using Quiniela.Services.Interfaces;
using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Quiniela.Services
{
    public class AuthService(IUserRepository userRepository, IRoleRepository roleRepository,
    IConfiguration config, CryptoHelper cryptoHelper) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IConfiguration _config = config;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailWithRoleAsync(request.Email);
            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return null;

            var token = GenerateJwtToken(user);
            var roleName = user.Role?.Name ?? "";

            return new LoginResponseDto
            {
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Role = new RoleDto { Name = roleName },
                Token = token
            };
        }

        public async Task<string> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("El email ya está en uso");

            var user = new User
            {
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = 2,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);
            return "Usuario registrado correctamente";
        }

        public async Task<string> ResetPasswordAsync(int id, ResetPasswordRequestDto request, int requestingUserId)
        {
            var requestingUser = await _userRepository.GetUserByIdAsync(requestingUserId);
            if (requestingUser == null)
                throw new UnauthorizedAccessException("Usuario no autorizado");

            bool isAdmin = requestingUser.RoleId == 1;  // SystemAdmin
            bool isOwnAccount = requestingUserId == id;

            if (!isAdmin && !isOwnAccount)
                throw new UnauthorizedAccessException("No tienes permiso para resetear la contraseña de otro usuario");

            var targetUser = await _userRepository.GetUserByIdAsync(id);
            if (targetUser == null)
                throw new InvalidOperationException("El usuario especificado no existe");

            var updated = await _userRepository.UpdatePasswordAsync(id, BCrypt.Net.BCrypt.HashPassword(request.NewPassword));
            if (!updated)
                throw new InvalidOperationException("Error al actualizar la contraseña");

            return "Contraseña actualizada correctamente";
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSetting = _config.GetSection("Jwt");
            var secretKey = jwtSetting["Key"]
                ?? throw new InvalidOperationException("JWT Key no está configurada en appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var roleName = user.Role?.Name ?? "";

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("Id", _cryptoHelper.Encrypt(user.Id.ToString()))
            };

            var expiresInMinutes = Convert.ToDouble(jwtSetting["ExpiresInMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: jwtSetting["Issuer"],
                audience: jwtSetting["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}