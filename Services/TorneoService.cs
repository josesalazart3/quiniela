using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class TorneoService(ITorneoRepository torneoRepository) : ITorneoService
    {
        private readonly ITorneoRepository _torneoRepository = torneoRepository;

        public async Task<TorneoReadDto> CreateTorneoAsync(TorneoCreateDto dto)
        {
            if (dto.FechaFin < dto.FechaInicio)
                throw new InvalidOperationException("La fecha de fin no puede ser menor que la fecha de inicio.");

            var torneo = new Torneo
            {
                Nombre = dto.Nombre,
                Año = dto.Año,
                PaisSede = dto.PaisSede,
                FechaInicio = DateTime.SpecifyKind(dto.FechaInicio, DateTimeKind.Utc),
                FechaFin = DateTime.SpecifyKind(dto.FechaFin, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _torneoRepository.AddTorneoAsync(torneo);
            return MapToReadDto(saved);
        }

        public async Task<IEnumerable<TorneoReadDto>> GetAllTorneosAsync(PaginacionDto paginacion)
        {
            var torneos = await _torneoRepository.GetAllTorneosAsync(paginacion.Page, paginacion.PageSize);
            return torneos.Select(MapToReadDto);
        }

        public async Task<TorneoReadDto?> GetTorneoByIdAsync(int id)
        {
            var torneo = await _torneoRepository.GetTorneoByIdAsync(id);
            if (torneo == null) return null;
            return MapToReadDto(torneo);
        }

        public async Task<TorneoReadDto?> UpdateTorneoAsync(int id, TorneoUpdateDto dto)
        {
            var torneo = await _torneoRepository.GetTorneoByIdAsync(id);
            if (torneo == null) return null;

            if (dto.FechaInicio.HasValue)
                torneo.FechaInicio = DateTime.SpecifyKind(dto.FechaInicio.Value, DateTimeKind.Utc);

            if (dto.FechaFin.HasValue)
                torneo.FechaFin = DateTime.SpecifyKind(dto.FechaFin.Value, DateTimeKind.Utc);

            if (torneo.FechaFin < torneo.FechaInicio)
                throw new InvalidOperationException("La fecha de fin no puede ser menor que la fecha de inicio.");

            torneo.Nombre = dto.Nombre ?? torneo.Nombre;
            torneo.PaisSede = dto.PaisSede ?? torneo.PaisSede;
            torneo.UpdatedAt = DateTime.UtcNow;

            var updated = await _torneoRepository.UpdateTorneoAsync(torneo);
            if (updated == null) return null;

            return MapToReadDto(updated);
        }

        public async Task<bool> DeleteTorneoAsync(int id)
        {
            return await _torneoRepository.DeleteTorneoAsync(id);
        }

        public async Task<IEnumerable<TorneoSelectDto>> GetTorneosSelectAsync()
        {
            var torneos = await _torneoRepository.GetTorneosSelectAsync();
            return torneos.Select(MapToSelectDto);
        }

        private static TorneoSelectDto MapToSelectDto(Torneo torneo) => new()
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre
        };

        private static TorneoReadDto MapToReadDto(Torneo torneo) => new()
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Año = torneo.Año,
            PaisSede = torneo.PaisSede,
            FechaInicio = torneo.FechaInicio,
            FechaFin = torneo.FechaFin,
            CreatedAt = torneo.CreatedAt
        };
    }
}