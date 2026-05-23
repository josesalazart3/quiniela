using Quiniela.Models;
using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class PartidoService(
        IPartidoRepository partidoRepository,
        ITorneoRepository torneoRepository,
        IFaseRepository faseRepository,
        IEstadioRepository estadioRepository,
        IClasificacionGrupoRepository clasificacionRepository,
        IPrediccionRepository prediccionRepository,
        BracketService bracketService,
        INotificacionService notificacionService) : IPartidoService
    {
        private readonly IPartidoRepository _partidoRepository = partidoRepository;
        private readonly ITorneoRepository _torneoRepository = torneoRepository;
        private readonly IFaseRepository _faseRepository = faseRepository;
        private readonly IEstadioRepository _estadioRepository = estadioRepository;
        private readonly IClasificacionGrupoRepository _clasificacionRepository = clasificacionRepository;
        private readonly IPrediccionRepository _prediccionRepository = prediccionRepository;
        private readonly INotificacionService _notificacionService = notificacionService;
        private readonly BracketService _bracketService = bracketService;


        public async Task<PartidoReadDto> CreatePartidoAsync(PartidoCreateDto dto)
        {
            var torneo = await _torneoRepository.GetTorneoByIdAsync(dto.TorneoId);
            if (torneo == null)
                throw new InvalidOperationException("El torneo especificado no existe");

            var fase = await _faseRepository.GetFaseByIdAsync(dto.FaseId);
            if (fase == null)
                throw new InvalidOperationException("La fase especificada no existe");

            var estadio = await _estadioRepository.GetEstadioByIdAsync(dto.EstadioId);
            if (estadio == null)
                throw new InvalidOperationException("El estadio especificado no existe");

            var partido = new Partido
            {
                TorneoId = dto.TorneoId,
                FaseId = dto.FaseId,
                GrupoId = dto.GrupoId,
                EquipoLocalId = dto.EquipoLocalId,
                EquipoVisitanteId = dto.EquipoVisitanteId,
                DescripcionLocal = dto.DescripcionLocal,
                DescripcionVisitante = dto.DescripcionVisitante,
                FechaHora = ConvertirAGuatemalaUtc(dto.FechaHora),
                EstadioId = dto.EstadioId,
                Finalizado = false
            };

            var saved = await _partidoRepository.AddPartidoAsync(partido);
            var savedWithDetails = await _partidoRepository.GetPartidoByIdWithDetailsAsync(saved.Id);
            return MapToReadDto(savedWithDetails!);
        }

        public async Task<IEnumerable<PartidoReadDto>> GetAllPartidosAsync(PaginacionDto paginacion)
        {
            var partidos = await _partidoRepository.GetAllPartidosAsync(paginacion.Page, paginacion.PageSize);
            return partidos.Select(MapToReadDto);
        }

        public async Task<IEnumerable<PartidoReadDto>> GetPartidosByTorneoAsync(int torneoId)
        {
            var partidos = await _partidoRepository.GetPartidosByTorneoAsync(torneoId);
            return partidos.Select(MapToReadDto);
        }

        public async Task<IEnumerable<PartidoReadDto>> GetPartidosByFaseAsync(int faseId)
        {
            var partidos = await _partidoRepository.GetPartidosByFaseAsync(faseId);
            return partidos.Select(MapToReadDto);
        }

        public async Task<IEnumerable<PartidoReadDto>> GetPartidosByGrupoAsync(int grupoId)
        {
            var partidos = await _partidoRepository.GetPartidosByGrupoAsync(grupoId);
            return partidos.Select(MapToReadDto);
        }

        public async Task<IEnumerable<PartidoReadDto>> GetPartidosPendientesAsync(int torneoId)
        {
            var partidos = await _partidoRepository.GetPartidosPendientesAsync(torneoId);
            return partidos.Select(MapToReadDto);
        }

        public async Task<PartidoReadDto?> GetPartidoByIdAsync(int id)
        {
            var partido = await _partidoRepository.GetPartidoByIdWithDetailsAsync(id);
            if (partido == null) return null;
            return MapToReadDto(partido);
        }

        public async Task<PartidoReadDto?> UpdatePartidoAsync(int id, PartidoUpdateDto dto)
        {
            var partido = await _partidoRepository.GetPartidoByIdAsync(id);
            if (partido == null) return null;

            partido.EquipoLocalId = dto.EquipoLocalId ?? partido.EquipoLocalId;
            partido.EquipoVisitanteId = dto.EquipoVisitanteId ?? partido.EquipoVisitanteId;
            partido.DescripcionLocal = dto.DescripcionLocal ?? partido.DescripcionLocal;
            partido.DescripcionVisitante = dto.DescripcionVisitante ?? partido.DescripcionVisitante;

            if (dto.FechaHora.HasValue)
                partido.FechaHora = ConvertirAGuatemalaUtc(dto.FechaHora.Value);

            partido.EstadioId = dto.EstadioId ?? partido.EstadioId;

            var updated = await _partidoRepository.UpdatePartidoAsync(partido);
            if (updated == null) return null;

            var updatedWithDetails = await _partidoRepository.GetPartidoByIdWithDetailsAsync(updated.Id);
            return MapToReadDto(updatedWithDetails!);
        }

        public async Task<PartidoReadDto?> IngresarResultadoAsync(int id, PartidoResultadoDto dto)
        {
            var partido = await _partidoRepository.GetPartidoByIdWithDetailsAsync(id);
            if (partido == null) return null;

            if (partido.Finalizado)
                throw new InvalidOperationException("El partido ya fue finalizado");

            // Validar penales — solo en eliminatorias con empate
            if (partido.FaseId > 1 &&
                dto.GolesLocal == dto.GolesVisitante &&
                (dto.GolesLocalPenales == null || dto.GolesVisitantePenales == null))
                throw new InvalidOperationException("En eliminatorias con empate debes ingresar el resultado de penales");

            var updated = await _partidoRepository.IngresarResultadoAsync(
                id,
                dto.GolesLocal,
                dto.GolesVisitante,
                dto.GolesLocalPenales,
                dto.GolesVisitantePenales
            );
            if (updated == null) return null;

            // 1. Actualizar clasificación si es fase de grupos
            if (partido.GrupoId.HasValue && partido.EquipoLocalId.HasValue && partido.EquipoVisitanteId.HasValue)
                await ActualizarClasificacionAsync(partido, dto.GolesLocal, dto.GolesVisitante);

            // 2. Calcular puntos de predicciones
            await CalcularPuntosPrediccionesAsync(id, dto.GolesLocal, dto.GolesVisitante);

            // 3. Actualizar bracket automáticamente
            await _bracketService.ActualizarBracketAsync(partido, dto.GolesLocal, dto.GolesVisitante, dto.GolesLocalPenales, dto.GolesVisitantePenales);

            // 4. Notificar SignalR
            await _notificacionService.NotificarResultadoPartidoAsync(partido.TorneoId, id);

            var predicciones = await _prediccionRepository.GetPrediccionesByPartidoAsync(id);
            var ligasAfectadas = predicciones.Select(p => p.LigaId).Distinct();
            foreach (var ligaId in ligasAfectadas)
                await _notificacionService.NotificarRankingLigaAsync(ligaId);

            var updatedWithDetails = await _partidoRepository.GetPartidoByIdWithDetailsAsync(id);
            return MapToReadDto(updatedWithDetails!);
        }

        public async Task<bool> DeletePartidoAsync(int id)
        {
            return await _partidoRepository.DeletePartidoAsync(id);
        }

        public async Task<PartidoReadDto?> ActualizarMarcadorAsync(int id, PartidoMarcadorDto dto)
        {
            var partido = await _partidoRepository.GetPartidoByIdWithDetailsAsync(id);
            if (partido == null) return null;

            if (partido.Finalizado)
                throw new InvalidOperationException("El partido ya fue finalizado");

            var updated = await _partidoRepository.ActualizarMarcadorAsync(id, dto.GolesLocal, dto.GolesVisitante);
            if (!updated) return null;

            await _notificacionService.NotificarResultadoPartidoAsync(partido.TorneoId, id);

            var updatedWithDetails = await _partidoRepository.GetPartidoByIdWithDetailsAsync(id);
            return MapToReadDto(updatedWithDetails!);
        }

        private async Task ActualizarClasificacionAsync(Partido partido, int golesLocal, int golesVisitante)
        {
            var clasificacionLocal = await _clasificacionRepository
                .GetClasificacionByGrupoYEquipoAsync(partido.GrupoId!.Value, partido.EquipoLocalId!.Value);

            var clasificacionVisitante = await _clasificacionRepository
                .GetClasificacionByGrupoYEquipoAsync(partido.GrupoId!.Value, partido.EquipoVisitanteId!.Value);

            if (clasificacionLocal == null || clasificacionVisitante == null) return;

            clasificacionLocal.PartidosJugados++;
            clasificacionVisitante.PartidosJugados++;

            clasificacionLocal.GolesAFavor += golesLocal;
            clasificacionLocal.GolesEnContra += golesVisitante;
            clasificacionVisitante.GolesAFavor += golesVisitante;
            clasificacionVisitante.GolesEnContra += golesLocal;

            clasificacionLocal.DiferenciaGoles = clasificacionLocal.GolesAFavor - clasificacionLocal.GolesEnContra;
            clasificacionVisitante.DiferenciaGoles = clasificacionVisitante.GolesAFavor - clasificacionVisitante.GolesEnContra;

            if (golesLocal > golesVisitante)
            {
                clasificacionLocal.Ganados++;
                clasificacionLocal.Puntos += 3;
                clasificacionVisitante.Perdidos++;
            }
            else if (golesLocal < golesVisitante)
            {
                clasificacionVisitante.Ganados++;
                clasificacionVisitante.Puntos += 3;
                clasificacionLocal.Perdidos++;
            }
            else
            {
                clasificacionLocal.Empatados++;
                clasificacionLocal.Puntos++;
                clasificacionVisitante.Empatados++;
                clasificacionVisitante.Puntos++;
            }

            await _clasificacionRepository.UpdateClasificacionAsync(clasificacionLocal);
            await _clasificacionRepository.UpdateClasificacionAsync(clasificacionVisitante);
        }

        private async Task CalcularPuntosPrediccionesAsync(int partidoId, int golesLocal, int golesVisitante)
        {
            await _prediccionRepository.ActualizarPuntosPrediccionesAsync(partidoId, golesLocal, golesVisitante);
        }

        private static DateTime ConvertirAGuatemalaUtc(DateTime fecha)
        {
            if (fecha.Kind == DateTimeKind.Utc)
                return fecha;

            var timeZoneId = OperatingSystem.IsWindows()
                ? "Central America Standard Time"
                : "America/Guatemala";

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var local = DateTime.SpecifyKind(fecha, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(local, tz);
        }

        private static PartidoReadDto MapToReadDto(Partido partido) => new()
        {
            Id = partido.Id,
            TorneoId = partido.TorneoId,
            Fase = partido.Fase == null ? null! : new FaseReadDto
            {
                Id = partido.Fase.Id,
                Nombre = partido.Fase.Nombre,
                Orden = partido.Fase.Orden,
                TorneoId = partido.Fase.TorneoId,
                TorneoNombre = partido.Fase.Torneo?.Nombre ?? string.Empty
            },
            GrupoId = partido.GrupoId,
            GrupoNombre = partido.Grupo?.Nombre,
            EquipoLocal = partido.EquipoLocal == null ? null : new EquipoReadDto
            {
                Id = partido.EquipoLocal.Id,
                Nombre = partido.EquipoLocal.Nombre,
                CodigoFifa = partido.EquipoLocal.CodigoFifa,
                BanderaUrl = partido.EquipoLocal.BanderaUrl,
                Entrenador = partido.EquipoLocal.Entrenador,
                Capitan = partido.EquipoLocal.Capitan
            },
            EquipoVisitante = partido.EquipoVisitante == null ? null : new EquipoReadDto
            {
                Id = partido.EquipoVisitante.Id,
                Nombre = partido.EquipoVisitante.Nombre,
                CodigoFifa = partido.EquipoVisitante.CodigoFifa,
                BanderaUrl = partido.EquipoVisitante.BanderaUrl,
                Entrenador = partido.EquipoVisitante.Entrenador,
                Capitan = partido.EquipoVisitante.Capitan
            },
            DescripcionLocal = partido.DescripcionLocal,
            DescripcionVisitante = partido.DescripcionVisitante,
            FechaHora = partido.FechaHora,
            Estadio = partido.Estadio == null ? null! : new EstadioReadDto
            {
                Id = partido.Estadio.Id,
                Nombre = partido.Estadio.Nombre,
                Ciudad = partido.Estadio.Ciudad,
                Pais = partido.Estadio.Pais,
                Capacidad = partido.Estadio.Capacidad
            },
            GolesLocal = partido.GolesLocal,
            GolesVisitante = partido.GolesVisitante,
            GolesLocalPenales = partido.GolesLocalPenales,
            GolesVisitantePenales = partido.GolesVisitantePenales,
            Finalizado = partido.Finalizado
        };
    }
}