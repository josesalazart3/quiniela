using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class GrupoService(
        IGrupoRepository grupoRepository,
        ITorneoRepository torneoRepository,
        IEquipoRepository equipoRepository,
        IClasificacionGrupoRepository clasificacionRepository) : IGrupoService
    {
        private readonly IGrupoRepository _grupoRepository = grupoRepository;
        private readonly ITorneoRepository _torneoRepository = torneoRepository;
        private readonly IEquipoRepository _equipoRepository = equipoRepository;
        private readonly IClasificacionGrupoRepository _clasificacionRepository = clasificacionRepository;

        public async Task<GrupoReadDto> CreateGrupoAsync(GrupoCreateDto dto)
        {
            var torneo = await _torneoRepository.GetTorneoByIdAsync(dto.TorneoId);
            if (torneo == null)
                throw new InvalidOperationException("El torneo especificado no existe");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new InvalidOperationException("El nombre del grupo es obligatorio");

            var grupo = new Grupo
            {
                Nombre = dto.Nombre.Trim(),
                TorneoId = dto.TorneoId,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _grupoRepository.AddGrupoAsync(grupo);
            var savedWithDetails = await _grupoRepository.GetGrupoByIdWithDetailsAsync(saved.Id);
            return MapToReadDto(savedWithDetails!);
        }

        public async Task<IEnumerable<GrupoReadDto>> GetAllGruposAsync(PaginacionDto paginacion)
        {
            var grupos = await _grupoRepository.GetAllGruposAsync(paginacion.Page, paginacion.PageSize);
            return grupos.Select(MapToReadDto);
        }

        public async Task<IEnumerable<GrupoReadDto>> GetGruposByTorneoAsync(int torneoId)
        {
            var grupos = await _grupoRepository.GetGruposByTorneoAsync(torneoId);
            return grupos.Select(MapToReadDto);
        }

        public async Task<IEnumerable<GrupoSelectDto>> GetGruposSelectAsync(int torneoId)
        {
            var grupos = await _grupoRepository.GetGruposSelectAsync(torneoId);
            return grupos.Select(MapToSelectDto);
        }

        public async Task<GrupoReadDto?> GetGrupoByIdAsync(int id)
        {
            var grupo = await _grupoRepository.GetGrupoByIdWithDetailsAsync(id);
            if (grupo == null) return null;
            return MapToReadDto(grupo);
        }

        public async Task<GrupoReadDto?> UpdateGrupoAsync(int id, GrupoUpdateDto dto)
        {
            var grupo = await _grupoRepository.GetGrupoByIdAsync(id);
            if (grupo == null) return null;

            if (dto.Nombre is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new InvalidOperationException("El nombre del grupo no puede estar vacío");

                grupo.Nombre = dto.Nombre.Trim();
            }

            grupo.UpdatedAt = DateTime.UtcNow;

            var updated = await _grupoRepository.UpdateGrupoAsync(grupo);
            if (updated == null) return null;

            var updatedWithDetails = await _grupoRepository.GetGrupoByIdWithDetailsAsync(updated.Id);
            return MapToReadDto(updatedWithDetails!);
        }

        public async Task<bool> DeleteGrupoAsync(int id)
        {
            return await _grupoRepository.DeleteGrupoAsync(id);
        }

        public async Task<GrupoReadDto> AsignarEquipoAGrupoAsync(int grupoId, int equipoId)
        {
            var grupo = await _grupoRepository.GetGrupoByIdAsync(grupoId);
            if (grupo == null)
                throw new InvalidOperationException("El grupo especificado no existe");

            var equipo = await _equipoRepository.GetEquipoByIdAsync(equipoId);
            if (equipo == null)
                throw new InvalidOperationException("El equipo especificado no existe");

            var yaExiste = await _grupoRepository.EquipoYaEnGrupoAsync(grupoId, equipoId);
            if (yaExiste)
                throw new InvalidOperationException("El equipo ya está asignado a este grupo");

            await _grupoRepository.AddEquipoAGrupoAsync(new GrupoEquipo
            {
                GrupoId = grupoId,
                EquipoId = equipoId
            });

            await _clasificacionRepository.AddClasificacionAsync(new ClasificacionGrupo
            {
                GrupoId = grupoId,
                EquipoId = equipoId
            });

            var grupoActualizado = await _grupoRepository.GetGrupoByIdWithDetailsAsync(grupoId);
            return MapToReadDto(grupoActualizado!);
        }

        public async Task<GrupoReadDto> AsignarVariosEquiposAGrupoAsync(int grupoId, GrupoEquipoAsignarVariosDto dto)
        {
            var grupo = await _grupoRepository.GetGrupoByIdAsync(grupoId);
            if (grupo == null)
                throw new InvalidOperationException("El grupo especificado no existe");

            var grupoEquipos = new List<GrupoEquipo>();
            var clasificaciones = new List<ClasificacionGrupo>();

            foreach (var equipoId in dto.EquipoIds)
            {
                var equipo = await _equipoRepository.GetEquipoByIdAsync(equipoId);
                if (equipo == null)
                    throw new InvalidOperationException($"El equipo con id {equipoId} no existe");

                var yaExiste = await _grupoRepository.EquipoYaEnGrupoAsync(grupoId, equipoId);
                if (yaExiste)
                    throw new InvalidOperationException($"El equipo {equipo.Nombre} ya está asignado a este grupo");

                grupoEquipos.Add(new GrupoEquipo
                {
                    GrupoId = grupoId,
                    EquipoId = equipoId
                });

                clasificaciones.Add(new ClasificacionGrupo
                {
                    GrupoId = grupoId,
                    EquipoId = equipoId
                });
            }

            await _grupoRepository.AsignarVariosEquiposAsync(grupoEquipos);

            foreach (var clasificacion in clasificaciones)
                await _clasificacionRepository.AddClasificacionAsync(clasificacion);

            var grupoActualizado = await _grupoRepository.GetGrupoByIdWithDetailsAsync(grupoId);
            return MapToReadDto(grupoActualizado!);
        }

        public async Task<bool> RemoverEquipoDeGrupoAsync(int grupoId, int equipoId)
        {
            var removido = await _grupoRepository.RemoveEquipoDeGrupoAsync(grupoId, equipoId);
            if (!removido) return false;

            await _clasificacionRepository.DeleteClasificacionAsync(grupoId, equipoId);
            return true;
        }

        public async Task<IEnumerable<ClasificacionGrupoReadDto>> GetClasificacionByGrupoAsync(int grupoId)
        {
            var clasificaciones = await _clasificacionRepository.GetClasificacionByGrupoAsync(grupoId);
            return clasificaciones.Select(c => new ClasificacionGrupoReadDto
            {
                Equipo = new EquipoReadDto
                {
                    Id = c.Equipo.Id,
                    Nombre = c.Equipo.Nombre,
                    CodigoFifa = c.Equipo.CodigoFifa,
                    BanderaUrl = c.Equipo.BanderaUrl,
                    Entrenador = c.Equipo.Entrenador,
                    Capitan = c.Equipo.Capitan
                },
                PartidosJugados = c.PartidosJugados,
                Ganados = c.Ganados,
                Empatados = c.Empatados,
                Perdidos = c.Perdidos,
                GolesAFavor = c.GolesAFavor,
                GolesEnContra = c.GolesEnContra,
                DiferenciaGoles = c.DiferenciaGoles,
                Puntos = c.Puntos
            });
        }

        private static GrupoReadDto MapToReadDto(Grupo grupo) => new()
        {
            Id = grupo.Id,
            Nombre = grupo.Nombre,
            TorneoId = grupo.TorneoId,
            TorneoNombre = grupo.Torneo?.Nombre ?? string.Empty,
            Equipos = grupo.Equipos?.Select(ge => new EquipoReadDto
            {
                Id = ge.Equipo.Id,
                Nombre = ge.Equipo.Nombre,
                CodigoFifa = ge.Equipo.CodigoFifa,
                BanderaUrl = ge.Equipo.BanderaUrl,
                Entrenador = ge.Equipo.Entrenador,
                Capitan = ge.Equipo.Capitan
            }).ToList() ?? new()
        };

        private static GrupoSelectDto MapToSelectDto(Grupo grupo) => new()
        {
            Id = grupo.Id,
            Nombre = grupo.Nombre
        };
    }
}