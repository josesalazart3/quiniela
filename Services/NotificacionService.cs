using Microsoft.AspNetCore.SignalR;
using Quiniela.Enums;
using Quiniela.Hubs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class NotificacionService(
        IHubContext<QuinielaHub> hubContext,
        IPartidoRepository partidoRepository,
        IClasificacionGrupoRepository clasificacionRepository,
        ILigaMiembroRepository ligaMiembroRepository) : INotificacionService
    {
        private readonly IHubContext<QuinielaHub> _hubContext = hubContext;
        private readonly IPartidoRepository _partidoRepository = partidoRepository;
        private readonly IClasificacionGrupoRepository _clasificacionRepository = clasificacionRepository;
        private readonly ILigaMiembroRepository _ligaMiembroRepository = ligaMiembroRepository;

        public async Task NotificarResultadoPartidoAsync(int torneoId, int partidoId)
        {
            var partido = await _partidoRepository.GetPartidoByIdWithDetailsAsync(partidoId);
            if (partido == null) return;

            await _hubContext.Clients
                .Group($"torneo_{torneoId}")
                .SendAsync("ResultadoActualizado", new
                {
                    partidoId,
                    golesLocal = partido.GolesLocal,
                    golesVisitante = partido.GolesVisitante,
                    finalizado = partido.Finalizado,
                    equipoLocalId = partido.EquipoLocalId,
                    equipoVisitanteId = partido.EquipoVisitanteId
                });

            if (partido.GrupoId.HasValue)
            {
                var clasificacion = await _clasificacionRepository
                    .GetClasificacionByGrupoAsync(partido.GrupoId.Value);

                await _hubContext.Clients
                    .Group($"torneo_{torneoId}")
                    .SendAsync("ClasificacionActualizada", new
                    {
                        grupoId = partido.GrupoId.Value,
                        clasificacion = clasificacion.Select(c => new
                        {
                            equipoId = c.EquipoId,
                            puntos = c.Puntos,
                            partidosJugados = c.PartidosJugados,
                            ganados = c.Ganados,
                            empatados = c.Empatados,
                            perdidos = c.Perdidos,
                            golesAFavor = c.GolesAFavor,
                            golesEnContra = c.GolesEnContra,
                            diferenciaGoles = c.DiferenciaGoles
                        })
                    });
            }
        }

        public async Task NotificarRankingLigaAsync(int ligaId)
        {
            var miembros = await _ligaMiembroRepository.GetMiembrosByLigaAsync(ligaId);

            await _hubContext.Clients
                .Group($"liga_{ligaId}")
                .SendAsync("RankingActualizado", new
                {
                    ligaId,
                    ranking = miembros
                        .Where(m => m.Estado == EstadoMiembro.Aprobado)
                        .OrderByDescending(m => m.Puntos)
                        .Select((m, index) => new
                        {
                            posicion = index + 1,
                            userId = m.UserId,
                            nombreEquipo = m.NombreEquipo,
                            puntos = m.Puntos
                        })
                });
        }
    }
}