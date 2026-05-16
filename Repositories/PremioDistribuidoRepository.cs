using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class PremioDistribuidoRepository(AppDbContext context) : IPremioDistribuidoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddRangeAsync(List<PremioDistribuido> premios)
        {
            await _context.PremiosDistribuidos.AddRangeAsync(premios);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PremioDistribuido>> GetByTorneoAsync(int torneoId)
        {
            return await _context.PremiosDistribuidos
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Liga)
                .Where(p => p.TorneoId == torneoId)
                .OrderBy(p => p.LigaId)
                .ThenBy(p => p.Posicion)
                .ToListAsync();
        }
    }
}