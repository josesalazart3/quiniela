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

        public async Task<IEnumerable<RankingGlobalUsuarioReadDto>> GetRankingGlobalUsuariosAsync()
        {
            var records = await GetGlobalUserBestRecordsAsync();
            return ToRankingGlobalUsuariosDto(records);
        }

        public async Task<IEnumerable<RankingGlobalLigaReadDto>> GetRankingGlobalLigasAsync()
        {
            var records = await GetGlobalLigaRecordsAsync();
            return ToRankingGlobalLigasDto(records);
        }

        public async Task<IEnumerable<RankingLigaReadDto>> GetPremiosLigaAsync(int ligaId)
        {
            var liga = await _context.Ligas
                .Include(l => l.LigaMiembros)
                    .ThenInclude(lm => lm.User)
                .FirstOrDefaultAsync(l => l.Id == ligaId);

            if (liga == null)
                return Enumerable.Empty<RankingLigaReadDto>();

            var miembros = liga.LigaMiembros
                .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                .OrderByDescending(lm => lm.Puntos)
                .ThenBy(lm => lm.UserId)
                .ToList();

            if (!miembros.Any())
                return Enumerable.Empty<RankingLigaReadDto>();

            if (!liga.EsDeApuestas)
            {
                return BuildCompetitionPositions(miembros, m => m.Puntos)
                    .Select(x => new RankingLigaReadDto
                    {
                        Posicion = x.Posicion,
                        UserId = x.Item.UserId,
                        FullName = GetFullName(x.Item.User),
                        NombreEquipo = x.Item.NombreEquipo,
                        Puntos = x.Item.Puntos,
                        PremioAsignado = null
                    })
                    .ToList();
            }

            var premioTotal = miembros.Count * (liga.PrecioPorUnirse ?? 0m) * 0.95m;
            var asignaciones = CalcularPremiosLiga(miembros, premioTotal, liga.Nombre);

            return asignaciones.Select(a => new RankingLigaReadDto
            {
                Posicion = a.Posicion,
                UserId = a.Miembro.UserId,
                FullName = GetFullName(a.Miembro.User),
                NombreEquipo = a.Miembro.NombreEquipo,
                Puntos = a.Miembro.Puntos,
                PremioAsignado = a.Premio
            }).ToList();
        }

        public async Task<PremiosGlobalesReadDto> GetPremiosGlobalesAsync()
        {
            var ligas = await _context.Ligas
                .AsNoTracking()
                .Include(l => l.LigaMiembros)
                .Where(l => l.EsDeApuestas)
                .ToListAsync();

            var totalGlobal = ligas.Sum(l =>
                l.LigaMiembros.Count(lm => lm.Estado == EstadoMiembro.Aprobado) *
                (l.PrecioPorUnirse ?? 0m));

            var montoGlobalIndividual = Math.Round(totalGlobal * 0.005m, 2);
            var montoGlobalLiga = Math.Round(totalGlobal * 0.005m, 2);

            var topIndividuales = await CalcularPremiosGlobalesIndividualesAsync(null, montoGlobalIndividual);

            var rankingLigas = await GetGlobalLigaRecordsAsync();
            RankingGlobalLigaReadDto? mejorLiga = null;

            var mejorLigaRecord = rankingLigas
                .OrderByDescending(x => x.PromedioPuntos)
                .ThenBy(x => x.LigaId)
                .FirstOrDefault();

            if (mejorLigaRecord != null)
            {
                var premioPerCapita = mejorLigaRecord.TotalMiembros > 0
                    ? Math.Round(montoGlobalLiga / mejorLigaRecord.TotalMiembros, 2)
                    : 0m;

                mejorLiga = new RankingGlobalLigaReadDto
                {
                    Posicion = 1,
                    LigaId = mejorLigaRecord.LigaId,
                    NombreLiga = mejorLigaRecord.NombreLiga,
                    PromedioPuntos = mejorLigaRecord.PromedioPuntos,
                    TotalMiembros = mejorLigaRecord.TotalMiembros,
                    PremioTotal = montoGlobalLiga,
                    PremioPerCapita = premioPerCapita
                };
            }

            return new PremiosGlobalesReadDto
            {
                TotalRecaudadoGlobal = totalGlobal,
                MontoGlobalIndividual = montoGlobalIndividual,
                MontoGlobalLiga = montoGlobalLiga,
                TopIndividuales = topIndividuales,
                MejorLiga = mejorLiga
            };
        }

        public async Task<IEnumerable<PremioDistribuidoReadDto>> CerrarTorneoYDistribuirPremiosAsync(int torneoId)
        {
            var torneo = await _torneoRepository.GetTorneoByIdAsync(torneoId);
            if (torneo == null)
                throw new InvalidOperationException("El torneo no existe");

            if (torneo.Finalizado)
                throw new InvalidOperationException("El torneo ya fue cerrado");

            var fechaDistribucion = DateTime.UtcNow;
            var premiosARegistrar = new List<PremioDistribuido>();

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
                    .ThenBy(lm => lm.UserId)
                    .ToList();

                if (!miembros.Any())
                    continue;

                var premioTotalLiga = miembros.Count * (liga.PrecioPorUnirse ?? 0m) * 0.95m;
                var asignacionesLiga = CalcularPremiosLiga(miembros, premioTotalLiga, liga.Nombre);

                foreach (var a in asignacionesLiga.Where(x => x.Premio.HasValue))
                {
                    premiosARegistrar.Add(new PremioDistribuido
                    {
                        TorneoId = torneoId,
                        UserId = a.Miembro.UserId,
                        LigaId = liga.Id,
                        Concepto = a.Concepto,
                        Monto = a.Premio!.Value,
                        Posicion = a.Posicion,
                        FechaDistribucion = fechaDistribucion
                    });
                }
            }

            var totalGlobal = ligas.Sum(l =>
                l.LigaMiembros.Count(lm => lm.Estado == EstadoMiembro.Aprobado) *
                (l.PrecioPorUnirse ?? 0m));

            var montoGlobalIndividual = Math.Round(totalGlobal * 0.005m, 2);
            var premiosGlobalesUsuarios = await CalcularPremiosGlobalesIndividualesAsync(torneoId, montoGlobalIndividual);

            foreach (var premio in premiosGlobalesUsuarios.Where(x => x.PremioAsignado.HasValue))
            {
                premiosARegistrar.Add(new PremioDistribuido
                {
                    TorneoId = torneoId,
                    UserId = premio.UserId,
                    LigaId = premio.LigaId,
                    Concepto = BuildConceptoGlobalIndividual(premio),
                    Monto = premio.PremioAsignado!.Value,
                    Posicion = premio.Posicion,
                    FechaDistribucion = fechaDistribucion
                });
            }

            var montoGlobalLiga = Math.Round(totalGlobal * 0.005m, 2);
            var rankingLigas = await GetGlobalLigaRecordsAsync(torneoId);
            var mejorLigaRecord = rankingLigas
                .OrderByDescending(x => x.PromedioPuntos)
                .ThenBy(x => x.LigaId)
                .FirstOrDefault();

            if (mejorLigaRecord != null)
            {
                var ligaGanadora = await _context.Ligas
                    .Include(l => l.LigaMiembros)
                    .FirstOrDefaultAsync(l => l.Id == mejorLigaRecord.LigaId);

                if (ligaGanadora != null)
                {
                    var miembrosLiga = ligaGanadora.LigaMiembros
                        .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                        .ToList();

                    if (miembrosLiga.Any())
                    {
                        var premioPerCapita = Math.Round(montoGlobalLiga / miembrosLiga.Count, 2);

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
            }

            await _premioRepository.AddRangeAsync(premiosARegistrar);

            torneo.Finalizado = true;
            torneo.UpdatedAt = DateTime.UtcNow;
            await _torneoRepository.UpdateTorneoAsync(torneo);

            var premiosGuardados = await _premioRepository.GetByTorneoAsync(torneoId);
            return premiosGuardados.Select(MapPremioToReadDto);
        }

        public async Task<IEnumerable<PremioDistribuidoReadDto>> GetPremiosDistribuidosAsync(int torneoId)
        {
            var premios = await _premioRepository.GetByTorneoAsync(torneoId);
            return premios.Select(MapPremioToReadDto);
        }

        private async Task<List<GlobalUserBestRecord>> GetGlobalUserBestRecordsAsync(int? torneoId = null)
        {
            var query = _context.LigaMiembros
                .AsNoTracking()
                .Include(lm => lm.User)
                .Include(lm => lm.Liga)
                .Where(lm =>
                    lm.Liga != null &&
                    lm.Liga.EsDeApuestas &&
                    lm.Estado == EstadoMiembro.Aprobado);

            if (torneoId.HasValue)
                query = query.Where(lm => lm.Liga!.TorneoId == torneoId.Value);

            var miembros = await query.ToListAsync();

            var mejores = miembros
                .GroupBy(lm => lm.UserId)
                .Select(g =>
                {
                    var mejor = g
                        .OrderByDescending(x => x.Puntos)
                        .ThenBy(x => x.LigaId)
                        .First();

                    return new GlobalUserBestRecord
                    {
                        UserId = mejor.UserId,
                        FullName = GetFullName(mejor.User),
                        TotalPuntos = mejor.Puntos,
                        LigaId = mejor.LigaId,
                        NombreLiga = mejor.Liga?.Nombre
                    };
                })
                .OrderByDescending(x => x.TotalPuntos)
                .ThenBy(x => x.UserId)
                .ToList();

            return mejores;
        }

        private async Task<List<GlobalLigaRecord>> GetGlobalLigaRecordsAsync(int? torneoId = null)
        {
            var query = _context.Ligas
                .AsNoTracking()
                .Include(l => l.LigaMiembros)
                .Where(l => l.EsDeApuestas);

            if (torneoId.HasValue)
                query = query.Where(l => l.TorneoId == torneoId.Value);

            var ligas = await query.ToListAsync();

            return ligas
                .Select(l =>
                {
                    var miembrosAprobados = l.LigaMiembros
                        .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                        .ToList();

                    var promedio = miembrosAprobados.Any()
                        ? miembrosAprobados.Average(lm => lm.Puntos)
                        : 0d;

                    return new GlobalLigaRecord
                    {
                        LigaId = l.Id,
                        NombreLiga = l.Nombre,
                        PromedioPuntos = Math.Round(promedio, 2),
                        TotalMiembros = miembrosAprobados.Count
                    };
                })
                .OrderByDescending(x => x.PromedioPuntos)
                .ThenBy(x => x.LigaId)
                .ToList();
        }

        private async Task<List<RankingGlobalUsuarioReadDto>> CalcularPremiosGlobalesIndividualesAsync(int? torneoId, decimal montoGlobalIndividual)
        {
            var ranking = await GetGlobalUserBestRecordsAsync(torneoId);
            if (!ranking.Any())
                return new List<RankingGlobalUsuarioReadDto>();

            var grupos = BuildCompetitionGroups(ranking, x => x.TotalPuntos);
            var resultado = new List<RankingGlobalUsuarioReadDto>();
            var premiosPorUserId = new Dictionary<int, decimal?>();

            foreach (var grupo in grupos)
                foreach (var item in grupo.Items)
                    premiosPorUserId[item.UserId] = null;

            var primerGrupo = grupos.FirstOrDefault(g => g.Posicion == 1);
            var segundoGrupo = grupos.FirstOrDefault(g => g.Posicion == 2);
            var tercerGrupo = grupos.FirstOrDefault(g => g.Posicion == 3);
            var tercerPremioConsumido = false;

            if (primerGrupo != null)
            {
                if (primerGrupo.Items.Count > 1)
                {
                    var premio = Math.Round(montoGlobalIndividual * 0.85m / primerGrupo.Items.Count, 2);
                    foreach (var item in primerGrupo.Items)
                        premiosPorUserId[item.UserId] = premio;
                }
                else
                {
                    premiosPorUserId[primerGrupo.Items[0].UserId] = Math.Round(montoGlobalIndividual * 0.50m, 2);
                }
            }

            if (primerGrupo == null || primerGrupo.Items.Count == 1)
            {
                if (segundoGrupo != null)
                {
                    if (segundoGrupo.Items.Count > 1)
                    {
                        var premio = Math.Round(montoGlobalIndividual * 0.35m / segundoGrupo.Items.Count, 2);
                        foreach (var item in segundoGrupo.Items)
                            premiosPorUserId[item.UserId] = premio;
                        tercerPremioConsumido = true;
                    }
                    else
                    {
                        premiosPorUserId[segundoGrupo.Items[0].UserId] = Math.Round(montoGlobalIndividual * 0.30m, 2);
                    }
                }

                if (!tercerPremioConsumido && tercerGrupo != null)
                {
                    //var premio = tercerGrupo.Items.Count > 1
                    //? Math.Round(montoGlobalIndividual * 0.10m / tercerGrupo.Items.Count, 2)
                    //: Math.Round(montoGlobalIndividual * 0.10m, 2);

                    var premio = tercerGrupo.Items.Count > 1
                        ? Math.Round(montoGlobalIndividual * 0.20m / tercerGrupo.Items.Count, 2)
                        : Math.Round(montoGlobalIndividual * 0.20m, 2);
                    foreach (var item in tercerGrupo.Items)
                        premiosPorUserId[item.UserId] = premio;
                }
            }

            foreach (var grupo in grupos.Where(g => g.Posicion <= 3))
            {
                foreach (var item in grupo.Items)
                {
                    resultado.Add(new RankingGlobalUsuarioReadDto
                    {
                        Posicion = grupo.Posicion,
                        UserId = item.UserId,
                        FullName = item.FullName,
                        TotalPuntos = item.TotalPuntos,
                        LigaId = item.LigaId,
                        NombreLiga = item.NombreLiga,
                        PremioAsignado = premiosPorUserId[item.UserId]
                    });
                }
            }

            return resultado;
        }

        private static List<LigaPrizeAssignment> CalcularPremiosLiga(List<LigaMiembro> miembros, decimal premioTotal, string nombreLiga)
        {
            var grupos = BuildCompetitionGroups(miembros, x => x.Puntos);
            var premiosPorUserId = miembros.ToDictionary(m => m.UserId, m => (decimal?)null);
            var conceptoPorUserId = miembros.ToDictionary(m => m.UserId, m => string.Empty);

            var primerGrupo = grupos.FirstOrDefault(g => g.Posicion == 1);
            var segundoGrupo = grupos.FirstOrDefault(g => g.Posicion == 2);
            var tercerGrupo = grupos.FirstOrDefault(g => g.Posicion == 3);

            // 1er lugar
            if (primerGrupo != null)
            {
                if (primerGrupo.Items.Count > 1)
                {
                    var premio = Math.Round(premioTotal * 0.85m / primerGrupo.Items.Count, 2);
                    foreach (var item in primerGrupo.Items)
                    {
                        premiosPorUserId[item.UserId] = premio;
                        conceptoPorUserId[item.UserId] = $"Empate 1° lugar — {nombreLiga}";
                    }
                }
                else
                {
                    var item = primerGrupo.Items[0];
                    premiosPorUserId[item.UserId] = Math.Round(premioTotal * 0.50m, 2);
                    conceptoPorUserId[item.UserId] = $"1° lugar — {nombreLiga}";

                    // 2do lugar — solo si no hay empate en 1ro
                    if (segundoGrupo != null)
                    {
                        if (segundoGrupo.Items.Count > 1)
                        {
                            var premio = Math.Round(premioTotal * 0.35m / segundoGrupo.Items.Count, 2);
                            foreach (var s in segundoGrupo.Items)
                            {
                                premiosPorUserId[s.UserId] = premio;
                                conceptoPorUserId[s.UserId] = $"Empate 2° lugar — {nombreLiga}";
                            }
                        }
                        else
                        {
                            var s = segundoGrupo.Items[0];
                            premiosPorUserId[s.UserId] = Math.Round(premioTotal * 0.25m, 2);
                            conceptoPorUserId[s.UserId] = $"2° lugar — {nombreLiga}";

                            // 3er lugar — solo si no hay empate en 2do
                            if (tercerGrupo != null)
                            {
                                if (tercerGrupo.Items.Count > 1)
                                {
                                    var premio = Math.Round(premioTotal * 0.10m / tercerGrupo.Items.Count, 2);
                                    foreach (var t in tercerGrupo.Items)
                                    {
                                        premiosPorUserId[t.UserId] = premio;
                                        conceptoPorUserId[t.UserId] = $"Empate 3° lugar — {nombreLiga}";
                                    }
                                }
                                else
                                {
                                    var t = tercerGrupo.Items[0];
                                    premiosPorUserId[t.UserId] = Math.Round(premioTotal * 0.10m, 2);
                                    conceptoPorUserId[t.UserId] = $"3° lugar — {nombreLiga}";
                                }
                            }
                        }
                    }
                }
            }

            // 4to lugar — SIEMPRE se evalua independientemente
            // Buscar quien esta en la posicion 4 del ranking real (considerando empates arriba)
            var cuartoGrupo = grupos
                .Where(g => !g.Items.Any(x => premiosPorUserId[x.UserId].HasValue))
                .Skip(grupos.Count(g => g.Items.Any(x => premiosPorUserId[x.UserId].HasValue)))
                .FirstOrDefault();

            // Simplificado: el primer grupo sin premio despues de los que ya tienen premio
            var gruposSinPremio = grupos
                .Where(g => !g.Items.Any(x => premiosPorUserId[x.UserId].HasValue))
                .ToList();

            // El 4to premio va al primer grupo que aun no tiene premio
            // SOLO si ya se repartieron los premios de arriba (hay al menos 1 ganador)
            var hayGanadores = premiosPorUserId.Any(x => x.Value.HasValue);
            if (hayGanadores && gruposSinPremio.Any())
            {
                var grupoCuartoPremio = gruposSinPremio.First();
                var premio4 = grupoCuartoPremio.Items.Count > 1
                    ? Math.Round(premioTotal * 0.10m / grupoCuartoPremio.Items.Count, 2)
                    : Math.Round(premioTotal * 0.10m, 2);

                foreach (var c in grupoCuartoPremio.Items)
                {
                    premiosPorUserId[c.UserId] = premio4;
                    conceptoPorUserId[c.UserId] = grupoCuartoPremio.Items.Count > 1
                        ? $"Empate 4° lugar — {nombreLiga}"
                        : $"4° lugar — {nombreLiga}";
                }
            }

            // Construir resultado final
            return grupos
                .SelectMany(g => g.Items.Select(item => new LigaPrizeAssignment
                {
                    Miembro = item,
                    Posicion = g.Posicion,
                    Premio = premiosPorUserId[item.UserId],
                    Concepto = conceptoPorUserId[item.UserId]
                }))
                .OrderBy(r => r.Posicion)
                .ThenByDescending(r => r.Miembro.Puntos)
                .ThenBy(r => r.Miembro.UserId)
                .ToList();
        }

        private static List<CompetitionGroup<T>> BuildCompetitionGroups<T>(
            List<T> items,
            Func<T, int> scoreSelector)
        {
            var grupos = new List<CompetitionGroup<T>>();
            var ordered = items.OrderByDescending(scoreSelector).ToList();
            int posicion = 1;

            foreach (var group in ordered.GroupBy(scoreSelector))
            {
                var groupItems = group.ToList();
                grupos.Add(new CompetitionGroup<T> { Posicion = posicion, Items = groupItems });
                posicion += groupItems.Count;
            }

            return grupos;
        }

        private static IEnumerable<CompetitionPosition<T>> BuildCompetitionPositions<T>(
            List<T> items,
            Func<T, int> scoreSelector)
        {
            var grupos = BuildCompetitionGroups(items, scoreSelector);
            return grupos.SelectMany(g => g.Items.Select(item => new CompetitionPosition<T>
            {
                Posicion = g.Posicion,
                Item = item
            }));
        }

        private static IEnumerable<RankingGlobalUsuarioReadDto> ToRankingGlobalUsuariosDto(List<GlobalUserBestRecord> records)
        {
            return BuildCompetitionPositions(records, x => x.TotalPuntos)
                .Select(x => new RankingGlobalUsuarioReadDto
                {
                    Posicion = x.Posicion,
                    UserId = x.Item.UserId,
                    FullName = x.Item.FullName,
                    TotalPuntos = x.Item.TotalPuntos,
                    LigaId = x.Item.LigaId,
                    NombreLiga = x.Item.NombreLiga,
                    PremioAsignado = null
                })
                .ToList();
        }

        private static IEnumerable<RankingGlobalLigaReadDto> ToRankingGlobalLigasDto(List<GlobalLigaRecord> records)
        {
            return BuildCompetitionPositions(records, x => (int)Math.Round(x.PromedioPuntos * 100))
                .Select(x => new RankingGlobalLigaReadDto
                {
                    Posicion = x.Posicion,
                    LigaId = x.Item.LigaId,
                    NombreLiga = x.Item.NombreLiga,
                    PromedioPuntos = x.Item.PromedioPuntos,
                    TotalMiembros = x.Item.TotalMiembros,
                    PremioTotal = null,
                    PremioPerCapita = null
                })
                .ToList();
        }

        private static string BuildConceptoGlobalIndividual(RankingGlobalUsuarioReadDto premio)
        {
            return premio.Posicion switch
            {
                1 when premio.PremioAsignado.HasValue => "1° lugar global individual",
                2 when premio.PremioAsignado.HasValue => "2° lugar global individual",
                3 when premio.PremioAsignado.HasValue => "3° lugar global individual",
                _ => "Premio global individual"
            };
        }

        private static string GetFullName(User? user)
        {
            if (user == null) return string.Empty;
            return $"{user.FirstName} {user.LastName}".Trim();
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

        private sealed class GlobalUserBestRecord
        {
            public int UserId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public int TotalPuntos { get; set; }
            public int? LigaId { get; set; }
            public string? NombreLiga { get; set; }
        }

        private sealed class GlobalLigaRecord
        {
            public int LigaId { get; set; }
            public string NombreLiga { get; set; } = string.Empty;
            public double PromedioPuntos { get; set; }
            public int TotalMiembros { get; set; }
        }

        private sealed class LigaPrizeAssignment
        {
            public LigaMiembro Miembro { get; set; } = null!;
            public int Posicion { get; set; }
            public decimal? Premio { get; set; }
            public string Concepto { get; set; } = string.Empty;
        }

        private sealed class CompetitionGroup<T>
        {
            public int Posicion { get; set; }
            public List<T> Items { get; set; } = new();
        }

        private sealed class CompetitionPosition<T>
        {
            public int Posicion { get; set; }
            public T Item { get; set; } = default!;
        }
    }
}