using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class FaseRepository(AppDbContext context) : IFaseRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Fase> AddFaseAsync(Fase fase)
        {
            _context.Fases.Add(fase);
            await _context.SaveChangesAsync();
            return fase;
        }

        public async Task<IEnumerable<Fase>> GetAllFasesAsync(int page, int pageSize)
        {
            return await _context.Fases
                .AsNoTracking()
                .Include(f => f.Torneo)
                .OrderBy(f => f.TorneoId)
                .ThenBy(f => f.Orden)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Fase>> GetFasesByTorneoAsync(int torneoId)
        {
            return await _context.Fases
                .AsNoTracking()
                .Include(f => f.Torneo)
                .Where(f => f.TorneoId == torneoId)
                .OrderBy(f => f.Orden)
                .ToListAsync();
        }

        public async Task<IEnumerable<Fase>> GetFasesSelectAsync(int torneoId)
        {
            return await _context.Fases
                .AsNoTracking()
                .Where(f => f.TorneoId == torneoId)
                .OrderBy(f => f.Orden)
                .ToListAsync();
        }

        public async Task<Fase?> GetFaseByIdAsync(int id)
        {
            return await _context.Fases
                .Include(f => f.Torneo)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Fase?> UpdateFaseAsync(Fase fase)
        {
            var existing = await _context.Fases.FindAsync(fase.Id);
            if (existing == null) return null;

            existing.Nombre = fase.Nombre;
            existing.Orden = fase.Orden;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteFaseAsync(int id)
        {
            var entity = await _context.Fases.FindAsync(id);
            if (entity == null) return false;

            _context.Fases.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}