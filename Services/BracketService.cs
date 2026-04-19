using Quiniela.Data;
using Quiniela.Enums;
using Quiniela.Models;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Services
{
    public class BracketService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        // Mapa de ganadores: partidoId → (nextPartidoId, esLocal)
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

            // Semifinales → Final y Tercer Puesto
            { 101, (104, true) }, { 102, (104, false) },
        };

        // Mapa de perdedores para tercer puesto
        private static readonly Dictionary<int, (int NextPartidoId, bool EsLocal)> MapaPerdedores = new()
        {
            { 101, (103, true)  },
            { 102, (103, false) },
        };

        // Mapa de grupos → partidos de dieciseisavos
        // Grupo → (PartidoId para 1ro, esLocal1ro, PartidoId para 2do, esLocal2do)
        private static readonly Dictionary<int, (int P1ro, bool Local1ro, int P2do, bool Local2do)> MapaGrupos = new()
        {
            { 1,  (79, true,  73, true)  }, // 1ºA → P79 local,  2ºA → P73 local
            { 2,  (85, true,  73, false) }, // 1ºB → P85 local,  2ºB → P73 visitante
            { 3,  (76, true,  75, false) }, // 1ºC → P76 local,  2ºC → P75 visitante
            { 4,  (81, true,  88, true)  }, // 1ºD → P81 local,  2ºD → P88 local
            { 5,  (74, true,  78, true)  }, // 1ºE → P74 local,  2ºE → P78 local
            { 6,  (75, true,  78, false) }, // 1ºF → P75 local,  2ºF → P78 visitante
            { 7,  (82, true,  88, false) }, // 1ºG → P82 local,  2ºG → P88 visitante
            { 8,  (84, true,  86, false) }, // 1ºH → P84 local,  2ºH → P86 visitante
            { 9,  (77, true,  78, false) }, // 1ºI → P77 local,  2ºI → P78 visitante -- ajustar si hay conflicto
            { 10, (86, true,  84, false) }, // 1ºJ → P86 local,  2ºJ → P84 visitante
            { 11, (87, true,  83, true)  }, // 1ºK → P87 local,  2ºK → P83 local
            { 12, (80, true,  83, false) }, // 1ºL → P80 local,  2ºL → P83 visitante
        };

        public async Task ActualizarBracketAsync(Partido partidoFinalizado, int golesLocal, int golesVisitante)
        {
            // Fase de grupos (FaseId = 1)
            if (partidoFinalizado.FaseId == 1 && partidoFinalizado.GrupoId.HasValue)
            {
                await ActualizarClasificacionYBracketGrupoAsync(partidoFinalizado);
                return;
            }

            // Fases eliminatorias
            await ActualizarBracketEliminatorioAsync(partidoFinalizado, golesLocal, golesVisitante);
        }

        private async Task ActualizarClasificacionYBracketGrupoAsync(Partido partido)
        {
            var grupoId = partido.GrupoId!.Value;

            // Verificar si todos los partidos del grupo están finalizados
            var partidos = await _context.Partidos
                .Where(p => p.GrupoId == grupoId)
                .ToListAsync();

            if (partidos.Any(p => !p.Finalizado)) return; // No todos terminaron

            // Obtener clasificación del grupo ordenada
            var clasificacion = await _context.ClasificacionGrupos
                .Where(c => c.GrupoId == grupoId)
                .OrderByDescending(c => c.Puntos)
                .ThenByDescending(c => c.DiferenciaGoles)
                .ThenByDescending(c => c.GolesAFavor)
                .ToListAsync();

            if (clasificacion.Count < 2) return;

            var primero = clasificacion[0];
            var segundo = clasificacion[1];

            if (!MapaGrupos.TryGetValue(grupoId, out var mapa)) return;

            // Actualizar partido del 1ro
            var partido1ro = await _context.Partidos.FindAsync(mapa.P1ro);
            if (partido1ro != null)
            {
                if (mapa.Local1ro)
                    partido1ro.EquipoLocalId = primero.EquipoId;
                else
                    partido1ro.EquipoVisitanteId = primero.EquipoId;
            }

            // Actualizar partido del 2do
            var partido2do = await _context.Partidos.FindAsync(mapa.P2do);
            if (partido2do != null)
            {
                if (mapa.Local2do)
                    partido2do.EquipoLocalId = segundo.EquipoId;
                else
                    partido2do.EquipoVisitanteId = segundo.EquipoId;
            }

            await _context.SaveChangesAsync();
        }

        private async Task ActualizarBracketEliminatorioAsync(Partido partido, int golesLocal, int golesVisitante)
        {
            // Determinar ganador y perdedor
            int? ganadorId = null;
            int? perdedorId = null;

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
            else
            {
                // En eliminatorias no hay empate — en caso real habría penales
                // Por ahora lo dejamos pendiente hasta implementar penales
                return;
            }

            // Actualizar siguiente partido con el ganador
            if (MapaGanadores.TryGetValue(partido.Id, out var siguienteGanador))
            {
                var nextPartido = await _context.Partidos.FindAsync(siguienteGanador.NextPartidoId);
                if (nextPartido != null)
                {
                    if (siguienteGanador.EsLocal)
                        nextPartido.EquipoLocalId = ganadorId;
                    else
                        nextPartido.EquipoVisitanteId = ganadorId;
                }
            }

            // Actualizar tercer puesto con el perdedor (solo semis)
            if (MapaPerdedores.TryGetValue(partido.Id, out var siguientePerdedor))
            {
                var nextPartido = await _context.Partidos.FindAsync(siguientePerdedor.NextPartidoId);
                if (nextPartido != null)
                {
                    if (siguientePerdedor.EsLocal)
                        nextPartido.EquipoLocalId = perdedorId;
                    else
                        nextPartido.EquipoVisitanteId = perdedorId;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}