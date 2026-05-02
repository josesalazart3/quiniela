using Quiniela.Models.DTOs;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services.Interfaces;
using Quiniela.Enums;
using Microsoft.EntityFrameworkCore;
using Quiniela.Data;

namespace Quiniela.Services
{
    public class RankingService(
        AppDbContext context,
        ILigaMiembroRepository ligaMiembroRepository,
        ILigaRepository ligaRepository) : IRankingService
    {
        private readonly AppDbContext _context = context;
        private readonly ILigaMiembroRepository _ligaMiembroRepository = ligaMiembroRepository;
        private readonly ILigaRepository _ligaRepository = ligaRepository;

        // Ranking global individual — toma el punteo más alto del usuario
        // entre todas las ligas de apuesta donde participa
        public async Task<IEnumerable<RankingGlobalUsuarioReadDto>> GetRankingGlobalUsuariosAsync()
        {
            var miembros = await _context.LigaMiembros
                .AsNoTracking()
                .Include(lm => lm.User)
                .Include(lm => lm.Liga)
                .Where(lm =>
                    lm.Liga != null &&
                    lm.Liga.EsDeApuestas &&
                    lm.Estado == EstadoMiembro.Aprobado)
                .ToListAsync();

            // Agrupar por usuario y tomar el punteo más alto
            var rankingUsuarios = miembros
                .GroupBy(lm => lm.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    FullName = $"{g.First().User?.FirstName} {g.First().User?.LastName}".Trim(),
                    TotalPuntos = g.Max(lm => lm.Puntos)
                })
                .OrderByDescending(r => r.TotalPuntos)
                .Select((r, index) => new RankingGlobalUsuarioReadDto
                {
                    Posicion = index + 1,
                    UserId = r.UserId,
                    FullName = r.FullName,
                    TotalPuntos = r.TotalPuntos,
                    PremioAsignado = null // se calcula en GetPremiosGlobalesAsync
                });

            return rankingUsuarios;
        }

        // Ranking global de ligas — ordenado por promedio de puntos
        // solo ligas de apuesta
        public async Task<IEnumerable<RankingGlobalLigaReadDto>> GetRankingGlobalLigasAsync()
        {
            var ligas = await _context.Ligas
                .AsNoTracking()
                .Include(l => l.LigaMiembros)
                .Where(l => l.EsDeApuestas)
                .ToListAsync();

            var rankingLigas = ligas
                .Select(l =>
                {
                    var miembrosAprobados = l.LigaMiembros
                        .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                        .ToList();

                    var promedio = miembrosAprobados.Any()
                        ? miembrosAprobados.Average(lm => lm.Puntos)
                        : 0;

                    var premioTotal = miembrosAprobados.Count * (l.PrecioPorUnirse ?? 0);

                    return new RankingGlobalLigaReadDto
                    {
                        LigaId = l.Id,
                        NombreLiga = l.Nombre,
                        PromedioPuntos = Math.Round(promedio, 2),
                        TotalMiembros = miembrosAprobados.Count,
                        PremioTotal = premioTotal,
                        PremioPerCapita = null // se calcula en GetPremiosGlobalesAsync
                    };
                })
                .OrderByDescending(r => r.PromedioPuntos)
                .Select((r, index) =>
                {
                    r.Posicion = index + 1;
                    return r;
                });

            return rankingLigas;
        }

        // Premios dentro de una liga específica
        public async Task<IEnumerable<RankingLigaReadDto>> GetPremiosLigaAsync(int ligaId)
        {
            var liga = await _context.Ligas
                .Include(l => l.LigaMiembros)
                    .ThenInclude(lm => lm.User)
                .FirstOrDefaultAsync(l => l.Id == ligaId);

            if (liga == null) return Enumerable.Empty<RankingLigaReadDto>();

            var miembros = liga.LigaMiembros
                .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                .OrderByDescending(lm => lm.Puntos)
                .ToList();

            if (!miembros.Any()) return Enumerable.Empty<RankingLigaReadDto>();

            var premioTotal = liga.EsDeApuestas
                ? miembros.Count * (liga.PrecioPorUnirse ?? 0) * 0.95m // 95% para repartir
                : 0;

            var resultado = new List<RankingLigaReadDto>();

            if (!liga.EsDeApuestas)
            {
                // Liga de diversión — solo ranking sin premios
                return miembros.Select((m, index) => new RankingLigaReadDto
                {
                    Posicion = index + 1,
                    UserId = m.UserId,
                    FullName = $"{m.User?.FirstName} {m.User?.LastName}".Trim(),
                    NombreEquipo = m.NombreEquipo,
                    Puntos = m.Puntos,
                    PremioAsignado = null
                });
            }

            // Liga de apuesta — calcular premios con reglas de empate
            var puntajeMax = miembros.Max(m => m.Puntos);
            var primerLugar = miembros.Where(m => m.Puntos == puntajeMax).ToList();

            if (primerLugar.Count > 1)
            {
                // Empate en primer lugar — 85% dividido equitativamente
                var premioEmpate = premioTotal * 0.85m / primerLugar.Count;
                return miembros.Select((m, index) => new RankingLigaReadDto
                {
                    Posicion = index + 1,
                    UserId = m.UserId,
                    FullName = $"{m.User?.FirstName} {m.User?.LastName}".Trim(),
                    NombreEquipo = m.NombreEquipo,
                    Puntos = m.Puntos,
                    PremioAsignado = primerLugar.Any(p => p.UserId == m.UserId)
                        ? Math.Round(premioEmpate, 2)
                        : null
                });
            }

            // Sin empate en primer lugar
            AsignarPremiosPorPosicion(miembros, premioTotal, resultado);
            return resultado;
        }

        // Premios globales — 1% del total recaudado entre todas las ligas de apuesta
        public async Task<PremiosGlobalesReadDto> GetPremiosGlobalesAsync()
        {
            var ligas = await _context.Ligas
                .AsNoTracking()
                .Include(l => l.LigaMiembros)
                .Where(l => l.EsDeApuestas)
                .ToListAsync();

            // Total recaudado global entre todas las ligas
            var totalGlobal = ligas.Sum(l =>
                l.LigaMiembros.Count(lm => lm.Estado == EstadoMiembro.Aprobado)
                * (l.PrecioPorUnirse ?? 0));

            var montoGlobal = totalGlobal * 0.01m;         // 1% del total
            var montoIndividual = montoGlobal * 0.5m;      // 0.5% para top 3 individuales
            var montoLiga = montoGlobal * 0.5m;            // 0.5% para mejor liga

            // Top 3 individuales
            var rankingIndividual = (await GetRankingGlobalUsuariosAsync()).Take(3).ToList();
            var premiosIndividuales = new List<RankingGlobalUsuarioReadDto>();

            if (rankingIndividual.Any())
            {
                // Verificar empates en top 3 y aplicar mismas reglas
                var top = rankingIndividual.ToList();
                var puntajeMax = top.Max(r => r.TotalPuntos);
                var empatadosPrimero = top.Where(r => r.TotalPuntos == puntajeMax).ToList();

                if (empatadosPrimero.Count > 1)
                {
                    var premioEmpate = montoIndividual * 0.85m / empatadosPrimero.Count;
                    premiosIndividuales = top.Select(r => new RankingGlobalUsuarioReadDto
                    {
                        Posicion = r.Posicion,
                        UserId = r.UserId,
                        FullName = r.FullName,
                        TotalPuntos = r.TotalPuntos,
                        PremioAsignado = empatadosPrimero.Any(e => e.UserId == r.UserId)
                            ? Math.Round(premioEmpate, 2)
                            : null
                    }).ToList();
                }
                else
                {
                    // Sin empate — mismas reglas 50%, 25%, 10%
                    var porcentajes = new[] { 0.50m, 0.25m, 0.10m };
                    premiosIndividuales = top.Select((r, index) => new RankingGlobalUsuarioReadDto
                    {
                        Posicion = r.Posicion,
                        UserId = r.UserId,
                        FullName = r.FullName,
                        TotalPuntos = r.TotalPuntos,
                        PremioAsignado = index < porcentajes.Length
                            ? Math.Round(montoIndividual * porcentajes[index], 2)
                            : null
                    }).ToList();
                }
            }

            // Mejor liga por promedio de puntos
            var rankingLigas = (await GetRankingGlobalLigasAsync()).FirstOrDefault();
            RankingGlobalLigaReadDto? mejorLiga = null;

            if (rankingLigas != null)
            {
                var ligaGanadora = await _context.Ligas
                    .Include(l => l.LigaMiembros)
                    .FirstOrDefaultAsync(l => l.Id == rankingLigas.LigaId);

                if (ligaGanadora != null)
                {
                    var totalMiembros = ligaGanadora.LigaMiembros
                        .Count(lm => lm.Estado == EstadoMiembro.Aprobado);

                    var premioPerCapita = totalMiembros > 0
                        ? Math.Round(montoLiga / totalMiembros, 2)
                        : 0;

                    mejorLiga = new RankingGlobalLigaReadDto
                    {
                        Posicion = 1,
                        LigaId = ligaGanadora.Id,
                        NombreLiga = ligaGanadora.Nombre,
                        PromedioPuntos = rankingLigas.PromedioPuntos,
                        TotalMiembros = totalMiembros,
                        PremioTotal = montoLiga,
                        PremioPerCapita = premioPerCapita
                    };
                }
            }

            return new PremiosGlobalesReadDto
            {
                TotalRecaudadoGlobal = totalGlobal,
                MontoGlobalIndividual = Math.Round(montoIndividual, 2),
                MontoGlobalLiga = Math.Round(montoLiga, 2),
                TopIndividuales = premiosIndividuales,
                MejorLiga = mejorLiga
            };
        }

        // =====================
        // HELPERS
        // =====================

        private static void AsignarPremiosPorPosicion(
            List<Models.LigaMiembro> miembros,
            decimal premioTotal,
            List<RankingLigaReadDto> resultado)
        {
            // Verificar empates en cada posición
            var grupos = miembros
                .GroupBy(m => m.Puntos)
                .OrderByDescending(g => g.Key)
                .ToList();

            int posicion = 1;
            var porcentajes = new[] { 0.50m, 0.25m, 0.10m, 0.10m }; // 1ro, 2do, 3ro, 4to

            foreach (var grupo in grupos)
            {
                var miembrosGrupo = grupo.ToList();
                decimal? premio = null;

                if (posicion <= 4)
                {
                    if (miembrosGrupo.Count > 1)
                    {
                        // Empate — repartir el porcentaje correspondiente equitativamente
                        premio = Math.Round(premioTotal * porcentajes[posicion - 1] / miembrosGrupo.Count, 2);
                    }
                    else
                    {
                        premio = Math.Round(premioTotal * porcentajes[posicion - 1], 2);

                    }
                }

                foreach (var miembro in miembrosGrupo)
                {
                    resultado.Add(new RankingLigaReadDto
                    {
                        Posicion = posicion,
                        UserId = miembro.UserId,
                        FullName = $"{miembro.User?.FirstName} {miembro.User?.LastName}".Trim(),
                        NombreEquipo = miembro.NombreEquipo,
                        Puntos = miembro.Puntos,
                        PremioAsignado = premio
                    });
                }

                posicion += miembrosGrupo.Count;
            }
        }
    }
}