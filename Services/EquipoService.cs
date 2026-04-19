using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class EquipoService(IEquipoRepository equipoRepository) : IEquipoService
    {
        private readonly IEquipoRepository _equipoRepository = equipoRepository;

        public async Task<EquipoReadDto> CreateEquipoAsync(EquipoCreateDto dto)
        {
            var equipo = new Equipo
            {
                Nombre = dto.Nombre,
                CodigoFifa = dto.CodigoFifa,
                BanderaUrl = dto.BanderaUrl,
                Entrenador = dto.Entrenador,
                Capitan = dto.Capitan
            };

            var saved = await _equipoRepository.AddEquipoAsync(equipo);
            return MapToReadDto(saved);
        }

        public async Task<IEnumerable<EquipoReadDto>> GetAllEquiposAsync(PaginacionDto paginacion)
        {
            var equipos = await _equipoRepository.GetAllEquiposAsync(paginacion.Page, paginacion.PageSize);
            return equipos.Select(MapToReadDto);
        }

        public async Task<IEnumerable<EquipoSelectDto>> GetEquiposSelectAsync()
        {
            var equipos = await _equipoRepository.GetEquiposSelectAsync();
            return equipos.Select(MapToSelectDto);
        }

        public async Task<EquipoReadDto?> GetEquipoByIdAsync(int id)
        {
            var equipo = await _equipoRepository.GetEquipoByIdAsync(id);
            if (equipo == null) return null;
            return MapToReadDto(equipo);
        }

        public async Task<EquipoReadDto?> UpdateEquipoAsync(int id, EquipoUpdateDto dto)
        {
            var equipo = await _equipoRepository.GetEquipoByIdAsync(id);
            if (equipo == null) return null;

            equipo.Nombre = dto.Nombre ?? equipo.Nombre;
            equipo.CodigoFifa = dto.CodigoFifa ?? equipo.CodigoFifa;
            equipo.BanderaUrl = dto.BanderaUrl ?? equipo.BanderaUrl;
            equipo.Entrenador = dto.Entrenador ?? equipo.Entrenador;
            equipo.Capitan = dto.Capitan ?? equipo.Capitan;

            var updated = await _equipoRepository.UpdateEquipoAsync(equipo);
            if (updated == null) return null;

            return MapToReadDto(updated);
        }

        public async Task<bool> DeleteEquipoAsync(int id)
        {
            return await _equipoRepository.DeleteEquipoAsync(id);
        }

        private static EquipoReadDto MapToReadDto(Equipo equipo) => new()
        {
            Id = equipo.Id,
            Nombre = equipo.Nombre,
            CodigoFifa = equipo.CodigoFifa,
            BanderaUrl = equipo.BanderaUrl,
            Entrenador = equipo.Entrenador,
            Capitan = equipo.Capitan
        };

        private static EquipoSelectDto MapToSelectDto(Equipo equipo) => new()
        {
            Id = equipo.Id,
            Nombre = equipo.Nombre,
            BanderaUrl = equipo.BanderaUrl,
            CodigoFifa = equipo.CodigoFifa
        };
    }
}