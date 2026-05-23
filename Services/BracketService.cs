using Quiniela.Data;
using Quiniela.Models;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Services
{
    public class BracketService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        // Mapa ganadores: partidoId → (nextPartidoId, esLocal)
        private static readonly Dictionary<int, (int NextPartidoId, bool EsLocal)> MapaGanadores = new()
        {
            // Dieciseisavos → Octavos
            { 74,  (89, true)  }, { 77,  (89, false) },
            { 73,  (90, true)  }, { 75,  (90, false) },
            { 76,  (91, true)  }, { 78,  (91, false) },
            { 79,  (92, true)  }, { 80,  (92, false) },
            { 83,  (93, true)  }, { 84,  (93, false) },
            { 81,  (94, true)  }, { 82,  (94, false) },
            { 86,  (95, true)  }, { 88,  (95, false) },
            { 85,  (96, true)  }, { 87,  (96, false) },

            // Octavos → Cuartos
            { 89,  (97, true)  }, { 90,  (97, false) },
            { 93,  (98, true)  }, { 94,  (98, false) },
            { 91,  (99, true)  }, { 92,  (99, false) },
            { 95,  (100, true) }, { 96,  (100, false) },

            // Cuartos → Semifinales
            { 97,  (101, true) }, { 98,  (101, false) },
            { 99,  (102, true) }, { 100, (102, false) },

            // Semifinales → Final
            { 101, (104, true) }, { 102, (104, false) },
        };

        // Perdedores de semis → Tercer Puesto
        private static readonly Dictionary<int, (int NextPartidoId, bool EsLocal)> MapaPerdedores = new()
        {
            { 101, (103, true)  },
            { 102, (103, false) },
        };

        // Mapa grupos → dieciseisavos (1ros y 2dos)
        private static readonly Dictionary<int, (int P1ro, bool Local1ro, int P2do, bool Local2do)> MapaGrupos = new()
        {
            { 1,  (79, true,  73, true)  },
            { 2,  (85, true,  73, false) },
            { 3,  (76, true,  75, false) },
            { 4,  (81, true,  88, true)  },
            { 5,  (74, true,  78, true)  },
            { 6,  (75, true,  78, false) },
            { 7,  (82, true,  88, false) },
            { 8,  (84, true,  86, false) },
            { 9,  (77, true,  78, false) },
            { 10, (86, true,  84, false) },
            { 11, (87, true,  83, true)  },
            { 12, (80, true,  83, false) },
        };

        public async Task ActualizarBracketAsync(Partido partido, int golesLocal, int golesVisitante, int? penalesLocal, int? penalesVisitante)
        {
            // Fase de grupos
            if (partido.FaseId == 1 && partido.GrupoId.HasValue)
            {
                await ActualizarBracketGrupoAsync(partido);
                return;
            }

            // Fases eliminatorias
            await ActualizarBracketEliminatorioAsync(partido, golesLocal, golesVisitante, penalesLocal, penalesVisitante);
        }

        private async Task ActualizarBracketGrupoAsync(Partido partido)
        {
            var grupoId = partido.GrupoId!.Value;

            // Verificar si todos los partidos del grupo terminaron
            var partidos = await _context.Partidos
                .Where(p => p.GrupoId == grupoId)
                .ToListAsync();

            if (partidos.Any(p => !p.Finalizado)) return;

            // Obtener clasificación ordenada
            var clasificacion = await _context.ClasificacionGrupos
                .Where(c => c.GrupoId == grupoId)
                .OrderByDescending(c => c.Puntos)
                .ThenByDescending(c => c.DiferenciaGoles)
                .ThenByDescending(c => c.GolesAFavor)
                .ToListAsync();

            if (clasificacion.Count < 2) return;
            if (!MapaGrupos.TryGetValue(grupoId, out var mapa)) return;

            // Asignar 1ro
            var partido1ro = await _context.Partidos.FindAsync(mapa.P1ro);
            if (partido1ro != null)
            {
                if (mapa.Local1ro) partido1ro.EquipoLocalId = clasificacion[0].EquipoId;
                else partido1ro.EquipoVisitanteId = clasificacion[0].EquipoId;
            }

            // Asignar 2do
            var partido2do = await _context.Partidos.FindAsync(mapa.P2do);
            if (partido2do != null)
            {
                if (mapa.Local2do) partido2do.EquipoLocalId = clasificacion[1].EquipoId;
                else partido2do.EquipoVisitanteId = clasificacion[1].EquipoId;
            }

            await _context.SaveChangesAsync();
        }

        private async Task ActualizarBracketEliminatorioAsync(Partido partido, int golesLocal, int golesVisitante, int? penalesLocal, int? penalesVisitante)
        {
            int? ganadorId;
            int? perdedorId;

            if (golesLocal > golesVisitante)
            {
                ganadorId = partido.EquipoLocalId;
                perdedorId = partido.EquipoVisitanteId;
            }
            else if (golesVisitante > golesLocal)
            {
                ganadorId = partido.EquipoVisitanteId;
                perdedorId = partido.EquipoLocalId;
            }
            else if (penalesLocal.HasValue && penalesVisitante.HasValue)
            {
                // Desempate por penales
                if (penalesLocal > penalesVisitante)
                {
                    ganadorId = partido.EquipoLocalId;
                    perdedorId = partido.EquipoVisitanteId;
                }
                else
                {
                    ganadorId = partido.EquipoVisitanteId;
                    perdedorId = partido.EquipoLocalId;
                }
            }
            else
            {
                // Empate sin penales — no avanzar
                return;
            }

            // Asignar ganador al siguiente partido
            if (MapaGanadores.TryGetValue(partido.Id, out var siguienteGanador))
            {
                var nextPartido = await _context.Partidos.FindAsync(siguienteGanador.NextPartidoId);
                if (nextPartido != null)
                {
                    if (siguienteGanador.EsLocal) nextPartido.EquipoLocalId = ganadorId;
                    else nextPartido.EquipoVisitanteId = ganadorId;
                }
            }

            // Asignar perdedor al tercer puesto (solo semis)
            if (MapaPerdedores.TryGetValue(partido.Id, out var siguientePerdedor))
            {
                var nextPartido = await _context.Partidos.FindAsync(siguientePerdedor.NextPartidoId);
                if (nextPartido != null)
                {
                    if (siguientePerdedor.EsLocal) nextPartido.EquipoLocalId = perdedorId;
                    else nextPartido.EquipoVisitanteId = perdedorId;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}