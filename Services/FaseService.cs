using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class FaseService(IFaseRepository faseRepository, ITorneoRepository torneoRepository) : IFaseService
    {
        private readonly IFaseRepository _faseRepository = faseRepository;
        private readonly ITorneoRepository _torneoRepository = torneoRepository;

        public async Task<FaseReadDto> CreateFaseAsync(FaseCreateDto dto)
        {
            var torneo = await _torneoRepository.GetTorneoByIdAsync(dto.TorneoId);
            if (torneo == null)
                throw new InvalidOperationException("El torneo especificado no existe");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new InvalidOperationException("El nombre de la fase es obligatorio");

            if (dto.Orden <= 0)
                throw new InvalidOperationException("El orden de la fase debe ser mayor que cero");

            var fase = new Fase
            {
                Nombre = dto.Nombre.Trim(),
                Orden = dto.Orden,
                TorneoId = dto.TorneoId,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _faseRepository.AddFaseAsync(fase);
            var savedWithDetails = await _faseRepository.GetFaseByIdAsync(saved.Id);
            return MapToReadDto(savedWithDetails!);
        }

        public async Task<IEnumerable<FaseReadDto>> GetAllFasesAsync(PaginacionDto paginacion)
        {
            var fases = await _faseRepository.GetAllFasesAsync(paginacion.Page, paginacion.PageSize);
            return fases.Select(MapToReadDto);
        }

        public async Task<IEnumerable<FaseReadDto>> GetFasesByTorneoAsync(int torneoId)
        {
            var fases = await _faseRepository.GetFasesByTorneoAsync(torneoId);
            return fases.Select(MapToReadDto);
        }

        public async Task<IEnumerable<FaseSelectDto>> GetFasesSelectAsync(int torneoId)
        {
            var fases = await _faseRepository.GetFasesSelectAsync(torneoId);
            return fases.Select(MapToSelectDto);
        }

        public async Task<FaseReadDto?> GetFaseByIdAsync(int id)
        {
            var fase = await _faseRepository.GetFaseByIdAsync(id);
            if (fase == null) return null;
            return MapToReadDto(fase);
        }

        public async Task<FaseReadDto?> UpdateFaseAsync(int id, FaseUpdateDto dto)
        {
            var fase = await _faseRepository.GetFaseByIdAsync(id);
            if (fase == null) return null;

            if (dto.Nombre is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new InvalidOperationException("El nombre de la fase no puede estar vacío");

                fase.Nombre = dto.Nombre.Trim();
            }

            if (dto.Orden.HasValue)
            {
                if (dto.Orden.Value <= 0)
                    throw new InvalidOperationException("El orden de la fase debe ser mayor que cero");

                fase.Orden = dto.Orden.Value;
            }

            fase.UpdatedAt = DateTime.UtcNow;

            var updated = await _faseRepository.UpdateFaseAsync(fase);
            if (updated == null) return null;

            var updatedWithDetails = await _faseRepository.GetFaseByIdAsync(updated.Id);
            return MapToReadDto(updatedWithDetails!);
        }

        public async Task<bool> DeleteFaseAsync(int id)
        {
            return await _faseRepository.DeleteFaseAsync(id);
        }

        private static FaseReadDto MapToReadDto(Fase fase) => new()
        {
            Id = fase.Id,
            Nombre = fase.Nombre,
            Orden = fase.Orden,
            TorneoId = fase.TorneoId,
            TorneoNombre = fase.Torneo?.Nombre ?? string.Empty
        };

        private static FaseSelectDto MapToSelectDto(Fase fase) => new()
        {
            Id = fase.Id,
            Nombre = fase.Nombre,
            Orden = fase.Orden
        };
    }
}