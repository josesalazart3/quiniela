using Microsoft.EntityFrameworkCore;
using Quiniela.Data;
using Quiniela.Enums;
using Quiniela.Models.DTOs;
using Quiniela.Services.Interfaces;
using Quiniela.Utils;
using Quiniela.Models;


namespace Quiniela.Services
{
    public class ReporteService(AppDbContext context) : IReporteService
    {
        private readonly AppDbContext _context = context;

        public async Task<ReporteResumenDto> GetResumenAsync()
        {
            var totalRecaudado = await _context.Ligas
                .Where(l => l.EsDeApuestas)
                .SelectMany(l => l.LigaMiembros)
                .Where(lm => lm.Estado == EstadoMiembro.Aprobado)
                .Select(lm => lm.Liga!.PrecioPorUnirse ?? 0)
                .SumAsync();

            return new ReporteResumenDto
            {
                TotalUsuarios = await _context.Users.CountAsync(),
                TotalLigas = await _context.Ligas.CountAsync(),
                TotalLigasApuesta = await _context.Ligas.CountAsync(l => l.EsDeApuestas),
                TotalLigasDiversion = await _context.Ligas.CountAsync(l => !l.EsDeApuestas),
                TotalPredicciones = await _context.Predicciones.CountAsync(),
                PartidosJugados = await _context.Partidos.CountAsync(p => p.Finalizado),
                PartidosPendientes = await _context.Partidos.CountAsync(p => !p.Finalizado),
                InvitacionesPendientes = await _context.InvitacionesLiga
                    .CountAsync(i => i.Estado == EstadoInvitacion.Pendiente),
                TotalRecaudado = totalRecaudado,
                TotalSesionesActivas = await _context.UserSessions
                    .CountAsync(s => s.Estado == EstadoSesion.Activa)
            };
        }

        public async Task<byte[]> ExportUsuariosCsvAsync()
        {
            var usuarios = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.LigaMiembros)
                .Include(u => u.Predicciones)
                .OrderBy(u => u.Id)
                .ToListAsync();

            var columns = new Dictionary<string, Func<User, object?>>
            {
                { "Id", u => u.Id },
                { "Email", u => u.Email },
                { "Nombre Completo", u => $"{u.FirstName} {u.LastName}".Trim() },
                { "Rol", u => u.Role?.Name ?? string.Empty },
                { "Fecha Registro", u => u.CreatedAt.ToString("yyyy-MM-dd HH:mm") },
                { "Total Ligas", u => u.LigaMiembros.Count },
                { "Total Predicciones", u => u.Predicciones.Count }
            };

            return CsvHelper.GenerateCsv(usuarios, columns);
        }

        public async Task<byte[]> ExportLigasCsvAsync()
        {
            var ligas = await _context.Ligas
                .AsNoTracking()
                .Include(l => l.CreatedByUser)
                .Include(l => l.LigaMiembros)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            var columns = new Dictionary<string, Func<Models.Liga, object?>>
            {
                { "Id", l => l.Id },
                { "Nombre", l => l.Nombre },
                { "Tipo", l => l.EsDeApuestas ? "Apuesta" : "Diversión" },
                { "Precio Entrada", l => l.PrecioPorUnirse?.ToString("F2") ?? "N/A" },
                { "Total Miembros", l => l.LigaMiembros.Count(lm => lm.Estado == EstadoMiembro.Aprobado) },
                { "Total Recaudado", l => l.EsDeApuestas
                    ? (l.LigaMiembros.Count(lm => lm.Estado == EstadoMiembro.Aprobado) * (l.PrecioPorUnirse ?? 0)).ToString("F2")
                    : "0.00" },
                { "Promedio Puntos", l => l.LigaMiembros.Any(lm => lm.Estado == EstadoMiembro.Aprobado)
                    ? l.LigaMiembros.Where(lm => lm.Estado == EstadoMiembro.Aprobado).Average(lm => lm.Puntos).ToString("F2")
                    : "0.00" },
                { "Creada Por", l => l.CreatedByUser == null ? string.Empty
                    : $"{l.CreatedByUser.FirstName} {l.CreatedByUser.LastName}".Trim() },
                { "Fecha Creación", l => l.CreatedAt.ToString("yyyy-MM-dd HH:mm") }
            };

            return CsvHelper.GenerateCsv(ligas, columns);
        }

        public async Task<byte[]> ExportPrediccionesCsvAsync(int ligaId)
        {
            var predicciones = await _context.Predicciones
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Liga)
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa!.EquipoLocal)
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa!.EquipoVisitante)
                .Where(p => p.LigaId == ligaId)
                .OrderBy(p => p.Partido!.FechaHora)
                .ToListAsync();

            var columns = new Dictionary<string, Func<Models.Prediccion, object?>>
            {
                { "Usuario", p => p.User == null ? string.Empty
                    : $"{p.User.FirstName} {p.User.LastName}".Trim() },
                { "Email", p => p.User?.Email ?? string.Empty },
                { "Partido", p => p.Partido == null ? string.Empty
                    : $"{p.Partido.EquipoLocal?.Nombre ?? p.Partido.DescripcionLocal} vs {p.Partido.EquipoVisitante?.Nombre ?? p.Partido.DescripcionVisitante}" },
                { "Fecha Partido", p => p.Partido?.FechaHora.ToString("yyyy-MM-dd HH:mm") ?? string.Empty },
                { "Predicción", p => $"{p.GolesLocal} - {p.GolesVisitante}" },
                { "Resultado Real", p => p.Partido != null && p.Partido.Finalizado
                    ? $"{p.Partido.GolesLocal} - {p.Partido.GolesVisitante}"
                    : "Pendiente" },
                { "Puntos Ganados", p => p.PuntosGanados }
            };

            return CsvHelper.GenerateCsv(predicciones, columns);
        }

        public async Task<byte[]> ExportPremiosDistribuidosCsvAsync(int torneoId)
        {
            var premios = await _context.PremiosDistribuidos
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Liga)
                .Where(p => p.TorneoId == torneoId)
                .OrderBy(p => p.LigaId)
                .ThenBy(p => p.Posicion)
                .ToListAsync();

            var columns = new Dictionary<string, Func<Models.PremioDistribuido, object?>>
            {
                { "Usuario", p => p.User == null ? string.Empty
                    : $"{p.User.FirstName} {p.User.LastName}".Trim() },
                { "Email", p => p.User?.Email ?? string.Empty },
                { "Liga", p => p.Liga?.Nombre ?? "Premio Global" },
                { "Concepto", p => p.Concepto },
                { "Posición", p => p.Posicion },
                { "Monto", p => p.Monto.ToString("F2") },
                { "Fecha", p => p.FechaDistribucion.ToString("yyyy-MM-dd HH:mm") }
            };

            return CsvHelper.GenerateCsv(premios, columns);
        }
    }
}