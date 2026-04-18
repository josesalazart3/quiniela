using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class TorneoRepository(AppDbContext context) : ITorneoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Torneo> AddTorneoAsync(Torneo torneo)
        {
            _context.Torneos.Add(torneo);
            await _context.SaveChangesAsync();
            return torneo;
        }

        public async Task<IEnumerable<Torneo>> GetAllTorneosAsync(int page, int pageSize)
        {
            return await _context.Torneos
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Torneo?> GetTorneoByIdAsync(int id)
        {
            return await _context.Torneos.FindAsync(id);
        }

        public async Task<Torneo?> GetTorneoByIdWithDetailsAsync(int id)
        {
            return await _context.Torneos
                .Include(t => t.Fases.OrderBy(f => f.Orden))
                .Include(t => t.Grupos)
                .Include(t => t.Partidos)
                    .ThenInclude(p => p.EquipoLocal)
                .Include(t => t.Partidos)
                    .ThenInclude(p => p.EquipoVisitante)
                .Include(t => t.Partidos)
                    .ThenInclude(p => p.Estadio)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Torneo?> UpdateTorneoAsync(Torneo torneo)
        {
            var existing = await _context.Torneos.FindAsync(torneo.Id);
            if (existing == null) return null;

            existing.Nombre = torneo.Nombre;
            existing.PaisSede = torneo.PaisSede;
            existing.FechaInicio = torneo.FechaInicio;
            existing.FechaFin = torneo.FechaFin;
            existing.UpdatedAt = torneo.UpdatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteTorneoAsync(int id)
        {
            var entity = await _context.Torneos.FindAsync(id);
            if (entity == null) return false;

            _context.Torneos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Torneo>> GetTorneosSelectAsync()
        {
            return await _context.Torneos
                .AsNoTracking()
                .OrderBy(t => t.Nombre)
                .ToListAsync();
        }
    }
}