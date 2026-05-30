using Quiniela.Services.Interfaces;
using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Quiniela.Services
{
    public class AuthService(IUserRepository userRepository, IRoleRepository roleRepository,
    IConfiguration config, CryptoHelper cryptoHelper, IUserSessionRepository sessionRepository, IPasswordResetTokenRepository passwordResetTokenRepository,
    IEmailService emailService, IHttpClientFactory httpClientFactory) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IConfiguration _config = config;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;
        private readonly IUserSessionRepository _sessionRepository = sessionRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository = passwordResetTokenRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;




        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request, string ipOrigen, string userAgent)
        {
            var user = await _userRepository.GetByEmailWithRoleAsync(request.Email);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return null;

            var jwtSetting = _config.GetSection("Jwt");
            var expiresInMinutes = Convert.ToDouble(jwtSetting["ExpiresInMinutes"] ?? "60");

            // Registrar sesión
            await _sessionRepository.CreateSessionAsync(new UserSession
            {
                UserId = user.Id,
                IpOrigen = ipOrigen,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                Estado = Enums.EstadoSesion.Activa
            });

            var token = GenerateJwtToken(user);
            var roleName = user.Role?.Name ?? string.Empty;

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

            bool isAdmin = requestingUser.RoleId == 1;  // Administrador
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

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdWithRoleAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Role = user.Role?.Name ?? string.Empty
            };
        }
        public async Task LogoutAsync(int userId)
        {
            var session = await _sessionRepository.GetActiveSessionAsync(userId);
            if (session != null)
                await _sessionRepository.CloseSessionAsync(session.Id);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            // Si el usuario no existe no revelamos esa información por seguridad
            if (user == null) return;

            // Invalidar tokens anteriores
            await _passwordResetTokenRepository.InvalidateAllUserTokensAsync(user.Id);

            // Crear nuevo token válido por 1 hora
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };

            await _passwordResetTokenRepository.CreateTokenAsync(resetToken);

            // Enviar email
            await _emailService.SendRecuperacionPasswordAsync(email, resetToken.Token);
        }

        public async Task RecoverPasswordAsync(RecoverPasswordDto dto)
        {
            var resetToken = await _passwordResetTokenRepository.GetValidTokenAsync(dto.Token);
            if (resetToken == null)
                throw new InvalidOperationException("El token es inválido o ha expirado");

            // Actualizar contraseña
            var updated = await _userRepository.UpdatePasswordAsync(
                resetToken.UserId,
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword)
            );

            if (!updated)
                throw new InvalidOperationException("Error al actualizar la contraseña");

            // Invalidar el token usado
            await _passwordResetTokenRepository.InvalidateTokenAsync(resetToken.Id);
        }

        public async Task<LoginResponseDto> GitHubLoginAsync(string code, string ipOrigen, string userAgent)
        {
            var accessToken = await ExchangeGithubCodeAsync(code);
            var (email, firstName, lastName) = await GetGithubUserInfoAsync(accessToken);

            var user = await _userRepository.GetByEmailWithRoleAsync(email);
            if (user == null)
            {
                var newUser = new User
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    RoleId = 2,
                    CreatedAt = DateTime.UtcNow
                };
                await _userRepository.AddUserAsync(newUser);
                user = await _userRepository.GetByEmailWithRoleAsync(email)!;
            }

            var jwtSetting = _config.GetSection("Jwt");
            var expiresInMinutes = Convert.ToDouble(jwtSetting["ExpiresInMinutes"] ?? "60");

            await _sessionRepository.CreateSessionAsync(new UserSession
            {
                UserId = user!.Id,
                IpOrigen = ipOrigen,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                Estado = Enums.EstadoSesion.Activa
            });

            return new LoginResponseDto
            {
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Role = new RoleDto { Name = user.Role?.Name ?? string.Empty },
                Token = GenerateJwtToken(user)
            };
        }

        private async Task<string> ExchangeGithubCodeAsync(string code)
        {
            var http = _httpClientFactory.CreateClient();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await http.PostAsJsonAsync("https://github.com/login/oauth/access_token", new
            {
                client_id = _config["GitHub:ClientId"],
                client_secret = _config["GitHub:ClientSecret"],
                code
            });

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (json.TryGetProperty("error", out var error))
                throw new InvalidOperationException($"GitHub OAuth error: {error.GetString()}");

            return json.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("GitHub no devolvió un access token");
        }

        private async Task<(string email, string firstName, string lastName)> GetGithubUserInfoAsync(string accessToken)
        {
            var http = _httpClientFactory.CreateClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            http.DefaultRequestHeaders.UserAgent.TryParseAdd("QuinielaApp");

            var userJson = await http.GetFromJsonAsync<JsonElement>("https://api.github.com/user");

            var fullName = userJson.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
            if (string.IsNullOrWhiteSpace(fullName) && userJson.TryGetProperty("login", out var loginProp))
                fullName = loginProp.GetString() ?? "GitHub User";

            var parts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var firstName = parts.Length > 0 ? parts[0] : fullName;
            var lastName = parts.Length > 1 ? parts[1] : "";

            var emailsJson = await http.GetFromJsonAsync<JsonElement[]>("https://api.github.com/user/emails");
            var email = emailsJson?
                .Where(e => e.TryGetProperty("verified", out var v) && v.GetBoolean())
                .OrderByDescending(e => e.TryGetProperty("primary", out var p) && p.GetBoolean())
                .Select(e => e.TryGetProperty("email", out var em) ? em.GetString() : null)
                .FirstOrDefault(e => e != null)
                ?? throw new InvalidOperationException("No se encontró un email verificado en la cuenta de GitHub");

            return (email, firstName, lastName);
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