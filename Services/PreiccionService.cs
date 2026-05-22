using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;
using Quiniela.Enums;

namespace Quiniela.Services
{
    public class PrediccionService(
        IPrediccionRepository prediccionRepository,
        IPartidoRepository partidoRepository,
        ILigaMiembroRepository ligaMiembroRepository) : IPrediccionService
    {
        private readonly IPrediccionRepository _prediccionRepository = prediccionRepository;
        private readonly IPartidoRepository _partidoRepository = partidoRepository;
        private readonly ILigaMiembroRepository _ligaMiembroRepository = ligaMiembroRepository;

        public async Task<PrediccionReadDto> CreatePrediccionAsync(PrediccionCreateDto dto, int userId)
        {
            // Verificar que el partido existe
            var partido = await _partidoRepository.GetPartidoByIdAsync(dto.PartidoId);
            if (partido == null)
                throw new InvalidOperationException("El partido especificado no existe");

            // Verificar que el partido no ha finalizado
            if (partido.Finalizado)
                throw new InvalidOperationException("No puedes predecir un partido que ya finalizó");

            // Verificar que faltan más de 15 minutos para el partido
            if (DateTime.UtcNow >= partido.FechaHora.AddMinutes(-15))
                throw new InvalidOperationException("El tiempo límite para predecir este partido ha expirado");

            // Verificar que el usuario es miembro aprobado de la liga
            var miembro = await _ligaMiembroRepository.GetMiembroAsync(userId, dto.LigaId);
            if (miembro == null || miembro.Estado != EstadoMiembro.Aprobado)
                throw new InvalidOperationException("Debes ser miembro aprobado de la liga para predecir");

            // Verificar que no existe ya una predicción
            var existente = await _prediccionRepository.GetPrediccionByUserLigaPartidoAsync(userId, dto.LigaId, dto.PartidoId);
            if (existente != null)
                throw new InvalidOperationException("Ya tienes una predicción para este partido en esta liga");

            var prediccion = new Prediccion
            {
                UserId = userId,
                LigaId = dto.LigaId,
                PartidoId = dto.PartidoId,
                GolesLocal = dto.GolesLocal,
                GolesVisitante = dto.GolesVisitante,
                PuntosGanados = 0,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _prediccionRepository.AddPrediccionAsync(prediccion);
            var savedWithDetails = await _prediccionRepository.GetPrediccionByIdAsync(saved.Id);
            return MapToReadDto(savedWithDetails!);
        }

        public async Task<IEnumerable<PrediccionReadDto>> GetPrediccionesByLigaAsync(int ligaId, PaginacionDto paginacion)
        {
            var predicciones = await _prediccionRepository.GetPrediccionesByLigaAsync(ligaId, paginacion.Page, paginacion.PageSize);
            return predicciones.Select(MapToReadDto);
        }

        public async Task<IEnumerable<PrediccionReadDto>> GetMisPrediccionesAsync(int userId, int ligaId, PaginacionDto paginacion)
        {
            var predicciones = await _prediccionRepository.GetPrediccionesByUserYLigaAsync(userId, ligaId, paginacion.Page, paginacion.PageSize);
            return predicciones.Select(MapToReadDto);
        }

        public async Task<PrediccionReadDto?> GetPrediccionByIdAsync(int id)
        {
            var prediccion = await _prediccionRepository.GetPrediccionByIdAsync(id);
            if (prediccion == null) return null;
            return MapToReadDto(prediccion);
        }

        public async Task<PrediccionReadDto?> UpdatePrediccionAsync(int id, PrediccionUpdateDto dto, int userId)
        {
            var prediccion = await _prediccionRepository.GetPrediccionByIdAsync(id);
            if (prediccion == null) return null;

            // Solo el dueño puede actualizar su predicción
            if (prediccion.UserId != userId)
                throw new UnauthorizedAccessException("No puedes modificar la predicción de otro usuario");

            // Verificar que el partido no ha finalizado
            if (prediccion.Partido.Finalizado)
                throw new InvalidOperationException("No puedes modificar la predicción de un partido que ya finalizó");

            // Verificar que faltan más de 15 minutos
            if (DateTime.UtcNow >= prediccion.Partido.FechaHora.AddMinutes(-15))
                throw new InvalidOperationException("El tiempo límite para modificar esta predicción ha expirado");

            prediccion.GolesLocal = dto.GolesLocal;
            prediccion.GolesVisitante = dto.GolesVisitante;
            prediccion.UpdatedAt = DateTime.UtcNow;

            var updated = await _prediccionRepository.UpdatePrediccionAsync(prediccion);
            if (updated == null) return null;

            var updatedWithDetails = await _prediccionRepository.GetPrediccionByIdAsync(updated.Id);
            return MapToReadDto(updatedWithDetails!);
        }

        public async Task<PrediccionReadDto?> GetPrediccionByUserLigaPartidoAsync(int userId, int ligaId, int partidoId)
        {
            var prediccion = await _prediccionRepository.GetPrediccionByUserLigaPartidoAsync(userId, ligaId, partidoId);
            if (prediccion == null) return null;
            return MapToReadDto(prediccion);
        }

        private static PrediccionReadDto MapToReadDto(Prediccion prediccion) => new()
        {
            Id = prediccion.Id,
            PartidoId = prediccion.PartidoId,
            LigaId = prediccion.LigaId,
            EquipoLocal = prediccion.Partido?.EquipoLocal?.Nombre ?? string.Empty,
            EquipoVisitante = prediccion.Partido?.EquipoVisitante?.Nombre ?? string.Empty,
            GolesLocal = prediccion.GolesLocal,
            GolesVisitante = prediccion.GolesVisitante,
            PuntosGanados = prediccion.PuntosGanados,
            Finalizado = prediccion.Partido?.Finalizado ?? false,
            FechaHora = prediccion.Partido?.FechaHora ?? DateTime.MinValue,
            CreatedAt = prediccion.CreatedAt,
            UpdatedAt = prediccion.UpdatedAt
        };
    }
}