using Quiniela.Models;
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
        ILigaRepository ligaRepository,
        ITorneoRepository torneoRepository,
        IPremioDistribuidoRepository premioRepository) : IRankingService
    {
        private readonly AppDbContext _context = context;
        private readonly ILigaMiembroRepository _ligaMiembroRepository = ligaMiembroRepository;
        private readonly ILigaRepository _ligaRepository = ligaRepository;
        private readonly ITorneoRepository _torneoRepository = torneoRepository;
        private readonly IPremioDistribuidoRepository _premioRepository = premioRepository;

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
                    PremioAsignado = null
                });

            return rankingUsuarios;
        }

        // Ranking global de ligas — ordenado por promedio de puntos solo ligas de apuesta
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
                        PremioPerCapita = null
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
                ? miembros.Count * (liga.PrecioPorUnirse ?? 0) * 0.95m
                : 0;

            var resultado = new List<RankingLigaReadDto>();

            if (!liga.EsDeApuestas)
            {
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

            var puntajeMax = miembros.Max(m => m.Puntos);
            var primerLugar = miembros.Where(m => m.Puntos == puntajeMax).ToList();

            if (primerLugar.Count > 1)
            {
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

            var totalGlobal = ligas.Sum(l =>
                l.LigaMiembros.Count(lm => lm.Estado == EstadoMiembro.Aprobado)
                * (l.PrecioPorUnirse ?? 0));

            var montoGlobal = totalGlobal * 0.01m;
            var montoIndividual = montoGlobal * 0.5m;
            var montoLiga = montoGlobal * 0.5m;

            var rankingIndividual = (await GetRankingGlobalUsuariosAsync()).Take(3).ToList();
            var premiosIndividuales = new List<RankingGlobalUsuarioReadDto>();

            if (rankingIndividual.Any())
            {
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

        // Cierra el torneo y registra los premios distribuidos oficialmente
        public async Task<IEnumerable<PremioDistribuidoReadDto>> CerrarTorneoYDistribuirPremiosAsync(int torneoId)
        {
            var torneo = await _torneoRepository.GetTorneoByIdAsync(torneoId);
            if (torneo == null)
                throw new InvalidOperationException("El torneo no existe");

            if (torneo.Finalizado)
                throw new InvalidOperationException("El torneo ya fue cerrado");

            var premiosARegistrar = new List<PremioDistribuido>();
            var fechaDistribucion = DateTime.UtcNow;

            // 1. Premios por liga
            var ligas = await _context.Ligas
                .Include(l => l.LigaMiembros)
                    .ThenInclude(lm => lm.User)
                .Where(l => l.TorneoId == torneoId && l.EsDeApuestas)
                .ToListAsync();

            foreach (var liga in ligas)
            {
                var miembros = liga.LigaMiembros
                    .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                    .OrderByDescending(lm => lm.Puntos)
                    .ToList();

                if (!miembros.Any()) continue;

                var premioTotal = miembros.Count * (liga.PrecioPorUnirse ?? 0) * 0.95m;
                var grupos = miembros.GroupBy(m => m.Puntos).OrderByDescending(g => g.Key).ToList();
                var porcentajes = new[] { 0.50m, 0.25m, 0.10m, 0.10m };
                int posicion = 1;

                foreach (var grupo in grupos)
                {
                    if (posicion > 4) break;
                    var miembrosGrupo = grupo.ToList();
                    decimal premio = miembrosGrupo.Count > 1
                        ? Math.Round(premioTotal * porcentajes[posicion - 1] / miembrosGrupo.Count, 2)
                        : Math.Round(premioTotal * porcentajes[posicion - 1], 2);

                    foreach (var miembro in miembrosGrupo)
                    {
                        premiosARegistrar.Add(new PremioDistribuido
                        {
                            TorneoId = torneoId,
                            UserId = miembro.UserId,
                            LigaId = liga.Id,
                            Concepto = $"{posicion}° lugar — {liga.Nombre}",
                            Monto = premio,
                            Posicion = posicion,
                            FechaDistribucion = fechaDistribucion
                        });
                    }
                    posicion += miembrosGrupo.Count;
                }
            }

            // 2. Premio global individual (0.5% del total recaudado)
            var totalGlobal = ligas.Sum(l =>
                l.LigaMiembros.Count(lm => lm.Estado == EstadoMiembro.Aprobado) *
                (l.PrecioPorUnirse ?? 0));

            var montoIndividual = totalGlobal * 0.005m;
            var rankingGlobal = (await GetRankingGlobalUsuariosAsync()).Take(3).ToList();
            var porcentajesGlobal = new[] { 0.50m, 0.25m, 0.10m };

            for (int i = 0; i < rankingGlobal.Count; i++)
            {
                premiosARegistrar.Add(new PremioDistribuido
                {
                    TorneoId = torneoId,
                    UserId = rankingGlobal[i].UserId,
                    LigaId = null,
                    Concepto = $"{i + 1}° lugar global individual",
                    Monto = Math.Round(montoIndividual * porcentajesGlobal[i], 2),
                    Posicion = i + 1,
                    FechaDistribucion = fechaDistribucion
                });
            }

            // 3. Premio global de liga (0.5% para la mejor liga)
            var montoLiga = totalGlobal * 0.005m;
            var mejorLiga = (await GetRankingGlobalLigasAsync()).FirstOrDefault();

            if (mejorLiga != null)
            {
                var ligaGanadora = await _context.Ligas
                    .Include(l => l.LigaMiembros)
                    .FirstOrDefaultAsync(l => l.Id == mejorLiga.LigaId);

                if (ligaGanadora != null)
                {
                    var miembrosLiga = ligaGanadora.LigaMiembros
                        .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                        .ToList();

                    var premioPerCapita = miembrosLiga.Any()
                        ? Math.Round(montoLiga / miembrosLiga.Count, 2)
                        : 0;

                    foreach (var miembro in miembrosLiga)
                    {
                        premiosARegistrar.Add(new PremioDistribuido
                        {
                            TorneoId = torneoId,
                            UserId = miembro.UserId,
                            LigaId = ligaGanadora.Id,
                            Concepto = $"Liga con mayor promedio — {ligaGanadora.Nombre}",
                            Monto = premioPerCapita,
                            Posicion = 1,
                            FechaDistribucion = fechaDistribucion
                        });
                    }
                }
            }

            // 4. Guardar premios y marcar torneo como finalizado
            await _premioRepository.AddRangeAsync(premiosARegistrar);

            torneo.Finalizado = true;
            torneo.UpdatedAt = DateTime.UtcNow;
            await _torneoRepository.UpdateTorneoAsync(torneo);

            return premiosARegistrar.Select(MapPremioToReadDto);
        }

        // Retorna el historial de premios distribuidos en un torneo
        public async Task<IEnumerable<PremioDistribuidoReadDto>> GetPremiosDistribuidosAsync(int torneoId)
        {
            var premios = await _premioRepository.GetByTorneoAsync(torneoId);
            return premios.Select(MapPremioToReadDto);
        }

        // =====================
        // HELPERS
        // =====================

        private static void AsignarPremiosPorPosicion(
            List<LigaMiembro> miembros,
            decimal premioTotal,
            List<RankingLigaReadDto> resultado)
        {
            var grupos = miembros
                .GroupBy(m => m.Puntos)
                .OrderByDescending(g => g.Key)
                .ToList();

            int posicion = 1;
            var porcentajes = new[] { 0.50m, 0.25m, 0.10m, 0.10m };

            foreach (var grupo in grupos)
            {
                var miembrosGrupo = grupo.ToList();
                decimal? premio = null;

                if (posicion <= 4)
                {
                    if (miembrosGrupo.Count > 1)
                        premio = Math.Round(premioTotal * porcentajes[posicion - 1] / miembrosGrupo.Count, 2);
                    else
                        premio = Math.Round(premioTotal * porcentajes[posicion - 1], 2);
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

        private static PremioDistribuidoReadDto MapPremioToReadDto(PremioDistribuido p) => new()
        {
            Id = p.Id,
            TorneoId = p.TorneoId,
            UserId = p.UserId,
            FullName = p.User == null ? string.Empty : $"{p.User.FirstName} {p.User.LastName}".Trim(),
            LigaId = p.LigaId,
            NombreLiga = p.Liga?.Nombre,
            Concepto = p.Concepto,
            Monto = p.Monto,
            Posicion = p.Posicion,
            FechaDistribucion = p.FechaDistribucion
        };
    }
}