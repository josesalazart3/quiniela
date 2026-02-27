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
            var user = await _userRepository.GetByUsernameWithRoleAsync(request.Username);
            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return null;

            var token = GenerateJwtToken(user);

            string roleName = "";
            if (user.Role != null)
            {
                roleName = user.Role.Name;
            }

            RoleDto roleDto = new()
            {
                Name = roleName
            };

            return new LoginResponseDto
            {
                Username = user.Username,
                Role = roleDto,
                Token = token
            };
        }

        public async Task<string?> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
                throw new InvalidOperationException("El username ya está en uso");


            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Username = request.Username,
                Password = hashedPassword,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = 2,
                CreatedAt = DateTime.UtcNow

            };

            await _userRepository.AddUserAsync(user);
            return "Usuario registrado correctamente";
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSetting = _config.GetSection("Jwt");
            var secretKey = jwtSetting["Key"]
                ?? throw new InvalidOperationException("JWT Key no está configurada en appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            string roleName = "";
            if (user.Role != null)
            {
                roleName = user.Role.Name;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
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

        public async Task<string> ResetPasswordAsync(int id, ResetPasswordRequestDto request, int userId) //-> userId por si se me anotoja que pueda actualizar algun admin
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("El usuario especificado no existe");

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;


            var updated = await _userRepository.UpdateUserAsync(user);
            if (updated == null)
                throw new InvalidOperationException("Error al actualizar el usuario");

            return "Password reseteada correctamente";
        }
    }
}