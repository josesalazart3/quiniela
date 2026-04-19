using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class LigaRepository(AppDbContext context) : ILigaRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Liga> AddLigaAsync(Liga liga)
        {
            _context.Ligas.Add(liga);
            await _context.SaveChangesAsync();
            return liga;
        }

        public async Task<IEnumerable<Liga>> GetAllLigasAsync(int page, int pageSize)
        {
            return await _context.Ligas
                .AsNoTracking()
                .Include(l => l.CreatedByUser)
                .Include(l => l.LigaMiembros)
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Liga>> GetLigasByUserAsync(int userId, int page, int pageSize)
        {
            return await _context.Ligas
                .AsNoTracking()
                .Include(l => l.CreatedByUser)
                .Include(l => l.LigaMiembros)
                .Where(l => l.LigaMiembros.Any(lm => lm.UserId == userId))
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Liga>> SearchLigasByNombreAsync(string nombre, int page, int pageSize)
        {
            return await _context.Ligas
                .AsNoTracking()
                .Include(l => l.CreatedByUser)
                .Include(l => l.LigaMiembros)
                .Where(l => l.Nombre.ToLower().Contains(nombre.ToLower()))
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Liga?> GetLigaByIdAsync(int id)
        {
            return await _context.Ligas.FindAsync(id);
        }

        public async Task<Liga?> GetLigaByIdWithDetailsAsync(int id)
        {
            return await _context.Ligas
                .Include(l => l.CreatedByUser)
                .Include(l => l.LigaMiembros)
                    .ThenInclude(lm => lm.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Liga?> UpdateLigaAsync(Liga liga)
        {
            var existing = await _context.Ligas.FindAsync(liga.Id);
            if (existing == null) return null;

            existing.Nombre = liga.Nombre;
            existing.EsDeApuestas = liga.EsDeApuestas;
            existing.PrecioPorUnirse = liga.PrecioPorUnirse;
            existing.UpdatedAt = liga.UpdatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteLigaAsync(int id)
	{
	    var liga = await _context.Ligas.FindAsync(id);
	    if (liga == null) return false;

	    liga.DeletedAt = DateTime.UtcNow;
	    await _context.SaveChangesAsync();
	    return true;
	}
    }
}
