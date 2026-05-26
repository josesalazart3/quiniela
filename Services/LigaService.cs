using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;
using Quiniela.Enums;

namespace Quiniela.Services
{
    public class LigaService(
        ILigaRepository ligaRepository,
        ILigaMiembroRepository ligaMiembroRepository,
        IUserRepository userRepository,
        IEmailService emailService) : ILigaService
    {
        private readonly ILigaRepository _ligaRepository = ligaRepository;
        private readonly ILigaMiembroRepository _ligaMiembroRepository = ligaMiembroRepository;
        private readonly IUserRepository _userRepository = userRepository;

        private readonly IEmailService _emailService = emailService;


        public async Task<LigaReadDto> CreateLigaAsync(LigaCreateDto dto, int userId)
        {
            // Validar que si es de apuestas tenga precio
            if (dto.EsDeApuestas && (dto.PrecioPorUnirse == null || dto.PrecioPorUnirse <= 0))
                throw new InvalidOperationException("Una liga de apuestas debe tener un precio de participación mayor a 0");

            // Validar que si no es de apuestas no tenga precio
            if (!dto.EsDeApuestas)
                dto.PrecioPorUnirse = null;

            var liga = new Liga
            {
                Nombre = dto.Nombre,
                EsDeApuestas = dto.EsDeApuestas,
                PrecioPorUnirse = dto.PrecioPorUnirse,
                TorneoId = dto.TorneoId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _ligaRepository.AddLigaAsync(liga);

            // Registrar al creador como admin automáticamente en estado Aprobado
            var miembro = new LigaMiembro
            {
                UserId = userId,
                LigaId = saved.Id,
                NombreEquipo = dto.NombreEquipo,
                EsAdmin = true,
                Puntos = 0,
                Estado = EstadoMiembro.Aprobado,
                FechaUnion = DateTime.UtcNow
            };

            await _ligaMiembroRepository.AddMiembroAsync(miembro);

            var savedWithDetails = await _ligaRepository.GetLigaByIdWithDetailsAsync(saved.Id);
            return MapToReadDto(savedWithDetails!);
        }

        public async Task<IEnumerable<LigaReadDto>> GetAllLigasAsync(PaginacionDto paginacion)
        {
            var ligas = await _ligaRepository.GetAllLigasAsync(paginacion.Page, paginacion.PageSize);
            return ligas.Select(MapToReadDto);
        }

        public async Task<IEnumerable<LigaReadDto>> GetMisLigasAsync(int userId, PaginacionDto paginacion)
        {
            var ligas = await _ligaRepository.GetLigasByUserAsync(userId, paginacion.Page, paginacion.PageSize);
            return ligas.Select(MapToReadDto);
        }

        public async Task<IEnumerable<LigaReadDto>> SearchLigasAsync(string nombre, PaginacionDto paginacion)
        {
            var ligas = await _ligaRepository.SearchLigasByNombreAsync(nombre, paginacion.Page, paginacion.PageSize);
            return ligas.Select(MapToReadDto);
        }

        public async Task<LigaReadDto?> GetLigaByIdAsync(int id)
        {
            var liga = await _ligaRepository.GetLigaByIdWithDetailsAsync(id);
            if (liga == null) return null;
            return MapToReadDto(liga);
        }

        public async Task<LigaReadDto?> UpdateLigaAsync(int id, LigaUpdateDto dto, int userId)
        {
            var liga = await _ligaRepository.GetLigaByIdAsync(id);
            if (liga == null) return null;

            // Solo el admin de la liga puede editarla
            var esAdmin = await _ligaMiembroRepository.EsAdminAsync(userId, id);
            if (!esAdmin)
                throw new UnauthorizedAccessException("Solo el administrador de la liga puede editarla");

            liga.Nombre = dto.Nombre ?? liga.Nombre;
            liga.EsDeApuestas = dto.EsDeApuestas ?? liga.EsDeApuestas;
            liga.PrecioPorUnirse = dto.PrecioPorUnirse ?? liga.PrecioPorUnirse;
            liga.UpdatedAt = DateTime.UtcNow;

            if (liga.EsDeApuestas && (liga.PrecioPorUnirse == null || liga.PrecioPorUnirse <= 0))
                throw new InvalidOperationException("Una liga de apuestas debe tener un precio de participación mayor a 0");

            if (!liga.EsDeApuestas)
                liga.PrecioPorUnirse = null;

            var updated = await _ligaRepository.UpdateLigaAsync(liga);
            if (updated == null) return null;

            var updatedWithDetails = await _ligaRepository.GetLigaByIdWithDetailsAsync(updated.Id);
            return MapToReadDto(updatedWithDetails!);
        }

        public async Task<bool> DeleteLigaAsync(int id, int userId)
        {
            var esAdmin = await _ligaMiembroRepository.EsAdminAsync(userId, id);
            if (!esAdmin)
                throw new UnauthorizedAccessException("Solo el administrador de la liga puede eliminarla");

            return await _ligaRepository.DeleteLigaAsync(id);
        }

        public async Task<LigaMiembroReadDto> UnirseALigaAsync(int ligaId, LigaMiembroCreateDto dto, int userId)
        {
            var liga = await _ligaRepository.GetLigaByIdAsync(ligaId);
            if (liga == null)
                throw new InvalidOperationException("La liga especificada no existe");

            var miembroExistente = await _ligaMiembroRepository.GetMiembroAsync(userId, ligaId);

            if (miembroExistente != null)
            {
                if (miembroExistente.Estado == EstadoMiembro.Aprobado ||
                    miembroExistente.Estado == EstadoMiembro.Pendiente)
                {
                    throw new InvalidOperationException("Ya tienes una solicitud activa o ya eres miembro de esta liga");
                }

                // Si estaba rechazado, reactivar la solicitud
                miembroExistente.NombreEquipo = dto.NombreEquipo;
                miembroExistente.EsAdmin = false;
                miembroExistente.Puntos = 0;
                miembroExistente.Estado = EstadoMiembro.Pendiente;
                miembroExistente.FechaUnion = DateTime.UtcNow;
                miembroExistente.DeletedAt = null;

                var actualizado = await _ligaMiembroRepository.UpdateMiembroAsync(miembroExistente);
                return MapMiembroToReadDto(actualizado!);
            }

            var miembro = new LigaMiembro
            {
                UserId = userId,
                LigaId = ligaId,
                NombreEquipo = dto.NombreEquipo,
                EsAdmin = false,
                Puntos = 0,
                Estado = EstadoMiembro.Pendiente,
                FechaUnion = DateTime.UtcNow
            };

            await _ligaMiembroRepository.AddMiembroAsync(miembro);
            var savedWithDetails = await _ligaMiembroRepository.GetMiembroAsync(userId, ligaId);
            return MapMiembroToReadDto(savedWithDetails!);
        }

        public async Task<LigaMiembroReadDto> AprobarMiembroAsync(int ligaId, LigaMiembroAprobacionDto dto, int adminId)
        {
            var esAdmin = await _ligaMiembroRepository.EsAdminAsync(adminId, ligaId);
            if (!esAdmin)
                throw new UnauthorizedAccessException("Solo el administrador de la liga puede aprobar miembros");

            var miembro = await _ligaMiembroRepository.GetMiembroAsync(dto.UserId, ligaId);
            if (miembro == null)
                throw new InvalidOperationException("No se encontró la solicitud del usuario en esta liga");

            if (miembro.Estado != EstadoMiembro.Pendiente)
                throw new InvalidOperationException("Solo se pueden aprobar o rechazar solicitudes pendientes");

            miembro.Estado = dto.Aprobar ? EstadoMiembro.Aprobado : EstadoMiembro.Rechazado;
            miembro.DeletedAt = null;

            var updated = await _ligaMiembroRepository.UpdateMiembroAsync(miembro);

            if (dto.Aprobar && miembro.User?.Email != null)
            {
                var liga = await _ligaRepository.GetLigaByIdAsync(ligaId);
                if (liga != null)
                    await _emailService.SendAprobacionMiembroAsync(miembro.User.Email, liga.Nombre);
            }

            return MapMiembroToReadDto(updated!);
        }

        public async Task<IEnumerable<LigaMiembroReadDto>> GetMiembrosByLigaAsync(int ligaId)
        {
            var miembros = await _ligaMiembroRepository.GetMiembrosByLigaAsync(ligaId);
            return miembros.Select(MapMiembroToReadDto);
        }

        public async Task<IEnumerable<LigaMiembroReadDto>> GetMiembrosPendientesAsync(int ligaId, int adminId)
        {
            var esAdmin = await _ligaMiembroRepository.EsAdminAsync(adminId, ligaId);
            if (!esAdmin)
                throw new UnauthorizedAccessException("Solo el administrador puede ver los miembros pendientes");

            var miembros = await _ligaMiembroRepository.GetMiembrosPendientesByLigaAsync(ligaId);
            return miembros.Select(MapMiembroToReadDto);
        }

        public async Task<bool> SalirDeLigaAsync(int ligaId, int userId)
        {
            var miembro = await _ligaMiembroRepository.GetMiembroAsync(userId, ligaId);
            if (miembro == null)
                throw new InvalidOperationException("No eres miembro de esta liga");

            if (miembro.EsAdmin)
                throw new InvalidOperationException("El administrador no puede salir de la liga");

            if (miembro.Estado != EstadoMiembro.Aprobado)
                throw new InvalidOperationException("Solo los miembros aprobados pueden salir de la liga");

            return await _ligaMiembroRepository.DeleteMiembroAsync(userId, ligaId);
        }

        public async Task<IEnumerable<RankingLigaReadDto>> GetRankingLigaAsync(int ligaId)
        {
            var miembros = await _ligaMiembroRepository.GetMiembrosByLigaAsync(ligaId);
            return miembros
                .Where(m => m.Estado == EstadoMiembro.Aprobado)
                .OrderByDescending(m => m.Puntos)
                .Select((m, index) => new RankingLigaReadDto
                {
                    Posicion = index + 1,
                    UserId = m.UserId,
                    FullName = $"{m.User?.FirstName} {m.User?.LastName}".Trim(),
                    NombreEquipo = m.NombreEquipo,
                    Puntos = m.Puntos,
                    PremioAsignado = null 
                });
        }

        private static LigaReadDto MapToReadDto(Liga liga) => new()
        {
            Id = liga.Id,
            Nombre = liga.Nombre,
            EsDeApuestas = liga.EsDeApuestas,
            PrecioPorUnirse = liga.PrecioPorUnirse,
            TorneoId = liga.TorneoId,
            CreadaPor = liga.CreatedByUser == null ? string.Empty
                : $"{liga.CreatedByUser.FirstName} {liga.CreatedByUser.LastName}".Trim(),
            TotalMiembros = liga.LigaMiembros?.Count(lm => lm.Estado == EstadoMiembro.Aprobado) ?? 0,
            FondoTotal = liga.EsDeApuestas
        ? liga.LigaMiembros?.Count(lm => lm.Estado == EstadoMiembro.Aprobado) * (liga.PrecioPorUnirse ?? 0) * 0.95m
        : null,
            CreatedAt = liga.CreatedAt
        };

        private static LigaMiembroReadDto MapMiembroToReadDto(LigaMiembro miembro) => new()
        {
            UserId = miembro.UserId,
            FullName = miembro.User == null ? string.Empty
                : $"{miembro.User.FirstName} {miembro.User.LastName}".Trim(),
            Email = miembro.User?.Email ?? string.Empty,
            NombreEquipo = miembro.NombreEquipo,
            EsAdmin = miembro.EsAdmin,
            Puntos = miembro.Puntos,
            Estado = miembro.Estado.ToString(),
            FechaUnion = miembro.FechaUnion
        };
    }
}