using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;
using Quiniela.Enums;

namespace Quiniela.Services
{
    public class InvitacionLigaService(
        IInvitacionLigaRepository invitacionRepository,
        ILigaRepository ligaRepository,
        ILigaMiembroRepository ligaMiembroRepository,
        IUserRepository userRepository,
        IEmailService emailService) : IInvitacionLigaService
    {
        private readonly IInvitacionLigaRepository _invitacionRepository = invitacionRepository;
        private readonly ILigaRepository _ligaRepository = ligaRepository;
        private readonly ILigaMiembroRepository _ligaMiembroRepository = ligaMiembroRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IEmailService _emailService = emailService;


        public async Task<InvitacionReadDto> EnviarInvitacionAsync(int ligaId, InvitacionCreateDto dto, int adminId)
        {
            // Verificar que quien invita es admin de la liga
            var esAdmin = await _ligaMiembroRepository.EsAdminAsync(adminId, ligaId);
            if (!esAdmin)
                throw new UnauthorizedAccessException("Solo el administrador puede enviar invitaciones");

            var liga = await _ligaRepository.GetLigaByIdAsync(ligaId);
            if (liga == null)
                throw new InvalidOperationException("La liga especificada no existe");

            // Verificar que no existe ya una invitación pendiente para ese email en esa liga
            var invitacionExistente = await _invitacionRepository.GetInvitacionByEmailYLigaAsync(dto.EmailInvitado, ligaId);
            if (invitacionExistente != null && invitacionExistente.Estado == EstadoInvitacion.Pendiente)
                throw new InvalidOperationException("Ya existe una invitación pendiente para ese email en esta liga");

            // Buscar si el email ya pertenece a un usuario registrado
            var usuarioExistente = await _userRepository.GetByEmailAsync(dto.EmailInvitado);

            // Si ya es miembro de la liga no tiene caso invitarlo
            if (usuarioExistente != null)
            {
                var yaMiembro = await _ligaMiembroRepository.EsMiembroAsync(usuarioExistente.Id, ligaId);
                if (yaMiembro)
                    throw new InvalidOperationException("El usuario ya es miembro de esta liga");
            }

            var invitacion = new InvitacionLiga
            {
                LigaId = ligaId,
                EmailInvitado = dto.EmailInvitado,
                UserId = usuarioExistente?.Id, // null si no está registrado
                Token = Guid.NewGuid().ToString(),
                Estado = EstadoInvitacion.Pendiente,
                FechaEnvio = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddDays(7) // expira en 7 días
            };

            var saved = await _invitacionRepository.AddInvitacionAsync(invitacion);

            await _emailService.SendInvitacionLigaAsync(
                dto.EmailInvitado,
                liga.Nombre,
                saved.Token
            );


            return MapToReadDto(saved);
        }

        public async Task<IEnumerable<InvitacionReadDto>> GetInvitacionesByLigaAsync(int ligaId, int adminId)
        {
            var esAdmin = await _ligaMiembroRepository.EsAdminAsync(adminId, ligaId);
            if (!esAdmin)
                throw new UnauthorizedAccessException("Solo el administrador puede ver las invitaciones");

            var invitaciones = await _invitacionRepository.GetInvitacionesByLigaAsync(ligaId);
            return invitaciones.Select(MapToReadDto);
        }

        public async Task<InvitacionReadDto?> ResponderInvitacionAsync(InvitacionResponderDto dto, int? userId)
        {
            var invitacion = await _invitacionRepository.GetInvitacionByTokenAsync(dto.Token);
            if (invitacion == null)
                throw new InvalidOperationException("Invitación no válida");

            if (invitacion.FechaExpiracion.HasValue && DateTime.UtcNow > invitacion.FechaExpiracion)
            {
                invitacion.Estado = EstadoInvitacion.Expirada;
                await _invitacionRepository.UpdateInvitacionAsync(invitacion);
                throw new InvalidOperationException("La invitación ha expirado");
            }

            if (invitacion.Estado != EstadoInvitacion.Pendiente)
                throw new InvalidOperationException("Esta invitación ya fue respondida");

            if (!dto.Aceptar)
            {
                invitacion.Estado = EstadoInvitacion.Rechazada;
                invitacion.FechaRespuesta = DateTime.UtcNow;
                await _invitacionRepository.UpdateInvitacionAsync(invitacion);
                return MapToReadDto(invitacion);
            }

            if (string.IsNullOrWhiteSpace(dto.NombreEquipo))
                throw new InvalidOperationException("Debes proporcionar un nombre de equipo para unirte");

            int miembroUserId;
            User? usuario;

            if (userId.HasValue)
            {
                usuario = await _userRepository.GetByEmailAsync(invitacion.EmailInvitado);
                if (usuario == null)
                    throw new InvalidOperationException("Debes registrarte antes de aceptar la invitación");

                if (usuario.Id != userId.Value)
                    throw new InvalidOperationException("Debes iniciar sesión con la cuenta que recibió la invitación");

                miembroUserId = usuario.Id;
            }
            else
            {
                usuario = await _userRepository.GetByEmailAsync(invitacion.EmailInvitado);
                if (usuario == null)
                    throw new InvalidOperationException("Debes registrarte antes de aceptar la invitación");

                miembroUserId = usuario.Id;
            }

            var miembroExistente = await _ligaMiembroRepository.GetMiembroAsync(miembroUserId, invitacion.LigaId);

            if (miembroExistente != null)
            {
                if (miembroExistente.Estado == EstadoMiembro.Aprobado ||
                    miembroExistente.Estado == EstadoMiembro.Pendiente)
                {
                    throw new InvalidOperationException("Ya tienes una solicitud activa o ya perteneces a la liga");
                }

                miembroExistente.NombreEquipo = dto.NombreEquipo;
                miembroExistente.EsAdmin = false;
                miembroExistente.Puntos = 0;
                miembroExistente.Estado = EstadoMiembro.Pendiente;
                miembroExistente.FechaUnion = DateTime.UtcNow;
                miembroExistente.DeletedAt = null;

                await _ligaMiembroRepository.UpdateMiembroAsync(miembroExistente);
            }
            else
            {
                await _ligaMiembroRepository.AddMiembroAsync(new LigaMiembro
                {
                    UserId = miembroUserId,
                    LigaId = invitacion.LigaId,
                    NombreEquipo = dto.NombreEquipo,
                    EsAdmin = false,
                    Puntos = 0,
                    Estado = EstadoMiembro.Pendiente,
                    FechaUnion = DateTime.UtcNow
                });
            }

            invitacion.Estado = EstadoInvitacion.Aceptada;
            invitacion.UserId = miembroUserId;
            invitacion.FechaRespuesta = DateTime.UtcNow;
            await _invitacionRepository.UpdateInvitacionAsync(invitacion);

            return MapToReadDto(invitacion);
        }

        public async Task<bool> CancelarInvitacionAsync(int invitacionId, int adminId)
        {
            var invitacion = await _invitacionRepository.GetInvitacionByIdAsync(invitacionId);
            if (invitacion == null) return false;

            var esAdmin = await _ligaMiembroRepository.EsAdminAsync(adminId, invitacion.LigaId);
            if (!esAdmin)
                throw new UnauthorizedAccessException("Solo el administrador puede cancelar invitaciones");

            return await _invitacionRepository.DeleteInvitacionAsync(invitacionId);
        }

        private static InvitacionReadDto MapToReadDto(InvitacionLiga invitacion) => new()
        {
            Id = invitacion.Id,
            EmailInvitado = invitacion.EmailInvitado,
            Estado = invitacion.Estado.ToString(),
            FechaEnvio = invitacion.FechaEnvio,
            FechaExpiracion = invitacion.FechaExpiracion,
            FechaRespuesta = invitacion.FechaRespuesta
        };
    }
}