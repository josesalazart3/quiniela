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
            var torneo = new Torneo
            {
                Nombre = dto.Nombre,
                Año = dto.Año,
                PaisSede = dto.PaisSede,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
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

            torneo.Nombre = dto.Nombre ?? torneo.Nombre;
            torneo.PaisSede = dto.PaisSede ?? torneo.PaisSede;
            torneo.FechaInicio = dto.FechaInicio ?? torneo.FechaInicio;
            torneo.FechaFin = dto.FechaFin ?? torneo.FechaFin;
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