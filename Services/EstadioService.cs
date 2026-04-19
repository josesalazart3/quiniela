using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class EstadioService(IEstadioRepository estadioRepository) : IEstadioService
    {
        private readonly IEstadioRepository _estadioRepository = estadioRepository;

        public async Task<EstadioReadDto> CreateEstadioAsync(EstadioCreateDto dto)
        {
            var estadio = new Estadio
            {
                Nombre = dto.Nombre,
                Ciudad = dto.Ciudad,
                Pais = dto.Pais,
                Capacidad = dto.Capacidad
            };

            var saved = await _estadioRepository.AddEstadioAsync(estadio);
            return MapToReadDto(saved);
        }

        public async Task<IEnumerable<EstadioReadDto>> GetAllEstadiosAsync(PaginacionDto paginacion)
        {
            var estadios = await _estadioRepository.GetAllEstadiosAsync(paginacion.Page, paginacion.PageSize);
            return estadios.Select(MapToReadDto);
        }

        public async Task<IEnumerable<EstadioSelectDto>> GetEstadiosSelectAsync()
        {
            var estadios = await _estadioRepository.GetEstadiosSelectAsync();
            return estadios.Select(MapToSelectDto);
        }

        public async Task<EstadioReadDto?> GetEstadioByIdAsync(int id)
        {
            var estadio = await _estadioRepository.GetEstadioByIdAsync(id);
            if (estadio == null) return null;
            return MapToReadDto(estadio);
        }

        public async Task<EstadioReadDto?> UpdateEstadioAsync(int id, EstadioUpdateDto dto)
        {
            var estadio = await _estadioRepository.GetEstadioByIdAsync(id);
            if (estadio == null) return null;

            estadio.Nombre = dto.Nombre ?? estadio.Nombre;
            estadio.Ciudad = dto.Ciudad ?? estadio.Ciudad;
            estadio.Pais = dto.Pais ?? estadio.Pais;
            estadio.Capacidad = dto.Capacidad ?? estadio.Capacidad;

            var updated = await _estadioRepository.UpdateEstadioAsync(estadio);
            if (updated == null) return null;

            return MapToReadDto(updated);
        }

        public async Task<bool> DeleteEstadioAsync(int id)
        {
            return await _estadioRepository.DeleteEstadioAsync(id);
        }

        private static EstadioReadDto MapToReadDto(Estadio estadio) => new()
        {
            Id = estadio.Id,
            Nombre = estadio.Nombre,
            Ciudad = estadio.Ciudad,
            Pais = estadio.Pais,
            Capacidad = estadio.Capacidad
        };

        private static EstadioSelectDto MapToSelectDto(Estadio estadio) => new()
        {
            Id = estadio.Id,
            Nombre = estadio.Nombre,
            //Ciudad = estadio.Ciudad
        };
    }
}