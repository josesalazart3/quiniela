using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class PartidoRepository(AppDbContext context) : IPartidoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Partido> AddPartidoAsync(Partido partido)
        {
            _context.Partidos.Add(partido);
            await _context.SaveChangesAsync();
            return partido;
        }

        public async Task<IEnumerable<Partido>> GetAllPartidosAsync(int page, int pageSize)
        {
            return await _context.Partidos
                .AsNoTracking()
                .Include(p => p.Fase)
                .Include(p => p.Grupo)
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Estadio)
                .OrderBy(p => p.FechaHora)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Partido>> GetPartidosByTorneoAsync(int torneoId)
        {
            return await _context.Partidos
                .AsNoTracking()
                .Include(p => p.Fase)
                .Include(p => p.Grupo)
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Estadio)
                .Where(p => p.TorneoId == torneoId)
                .OrderBy(p => p.FechaHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Partido>> GetPartidosByFaseAsync(int faseId)
        {
            return await _context.Partidos
                .AsNoTracking()
                .Include(p => p.Fase)
                .Include(p => p.Grupo)
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Estadio)
                .Where(p => p.FaseId == faseId)
                .OrderBy(p => p.FechaHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Partido>> GetPartidosByGrupoAsync(int grupoId)
        {
            return await _context.Partidos
                .AsNoTracking()
                .Include(p => p.Fase)
                .Include(p => p.Grupo)
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Estadio)
                .Where(p => p.GrupoId == grupoId)
                .OrderBy(p => p.FechaHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Partido>> GetPartidosPendientesAsync(int torneoId)
        {
            return await _context.Partidos
                .AsNoTracking()
                .Include(p => p.Fase)
                .Include(p => p.Grupo)
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Estadio)
                .Where(p => p.TorneoId == torneoId && !p.Finalizado)
                .OrderBy(p => p.FechaHora)
                .ToListAsync();
        }

        public async Task<Partido?> GetPartidoByIdAsync(int id)
        {
            return await _context.Partidos.FindAsync(id);
        }

        public async Task<Partido?> GetPartidoByIdWithDetailsAsync(int id)
        {
            return await _context.Partidos
                .Include(p => p.Fase)
                .Include(p => p.Grupo)
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Estadio)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Partido?> UpdatePartidoAsync(Partido partido)
        {
            var existing = await _context.Partidos.FindAsync(partido.Id);
            if (existing == null) return null;

            existing.EquipoLocalId = partido.EquipoLocalId;
            existing.EquipoVisitanteId = partido.EquipoVisitanteId;
            existing.DescripcionLocal = partido.DescripcionLocal;
            existing.DescripcionVisitante = partido.DescripcionVisitante;
            existing.FechaHora = partido.FechaHora;
            existing.EstadioId = partido.EstadioId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Partido?> IngresarResultadoAsync(int id, int golesLocal, int golesVisitante)
        {
            var existing = await _context.Partidos.FindAsync(id);
            if (existing == null) return null;

            existing.GolesLocal = golesLocal;
            existing.GolesVisitante = golesVisitante;
            existing.Finalizado = true;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeletePartidoAsync(int id)
        {
            var partido = await _context.Partidos.FindAsync(id);
            if (partido == null) return false;

            partido.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Partido>> GetPartidosFinalizadosByGrupoAsync(int grupoId)
        {
            return await _context.Partidos
                .AsNoTracking()
                .Where(p => p.GrupoId == grupoId && p.Finalizado)
                .ToListAsync();
        }

        public async Task<bool> ActualizarMarcadorAsync(int id, int golesLocal, int golesVisitante)
        {
            var partido = await _context.Partidos.FindAsync(id);
            if (partido == null) return false;

            partido.GolesLocal = golesLocal;
            partido.GolesVisitante = golesVisitante;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
