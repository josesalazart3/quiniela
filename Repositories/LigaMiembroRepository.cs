using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class LigaMiembroRepository(AppDbContext context) : ILigaMiembroRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<LigaMiembro> AddMiembroAsync(LigaMiembro miembro)
        {
            _context.LigaMiembros.Add(miembro);
            await _context.SaveChangesAsync();
            return miembro;
        }

        public async Task<IEnumerable<LigaMiembro>> GetMiembrosByLigaAsync(int ligaId)
        {
            return await _context.LigaMiembros
                .AsNoTracking()
                .Include(lm => lm.User)
                .Where(lm => lm.LigaId == ligaId)
                .OrderByDescending(lm => lm.Puntos)
                .ToListAsync();
        }

        public async Task<IEnumerable<LigaMiembro>> GetMiembrosPendientesByLigaAsync(int ligaId)
        {
            return await _context.LigaMiembros
                .AsNoTracking()
                .Include(lm => lm.User)
                .Where(lm => lm.LigaId == ligaId && lm.Estado == Enums.EstadoMiembro.Pendiente)
                .ToListAsync();
        }

        public async Task<LigaMiembro?> GetMiembroAsync(int userId, int ligaId)
        {
            return await _context.LigaMiembros
                .Include(lm => lm.User)
                .FirstOrDefaultAsync(lm => lm.UserId == userId && lm.LigaId == ligaId);
        }

        public async Task<LigaMiembro?> UpdateMiembroAsync(LigaMiembro miembro)
        {
            var existing = await _context.LigaMiembros
                .FirstOrDefaultAsync(lm => lm.UserId == miembro.UserId && lm.LigaId == miembro.LigaId);
            if (existing == null) return null;

            existing.NombreEquipo = miembro.NombreEquipo;
            existing.EsAdmin = miembro.EsAdmin;
            existing.Puntos = miembro.Puntos;
            existing.Estado = miembro.Estado;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteMiembroAsync(int userId, int ligaId)
	{
	    var miembro = await _context.LigaMiembros
		.FirstOrDefaultAsync(lm => lm.UserId == userId && lm.LigaId == ligaId);
	    if (miembro == null) return false;

	    miembro.DeletedAt = DateTime.UtcNow;
	    await _context.SaveChangesAsync();
	    return true;
	}

        public async Task<bool> EsMiembroAsync(int userId, int ligaId)
        {
            return await _context.LigaMiembros
                .AnyAsync(lm => lm.UserId == userId && lm.LigaId == ligaId);
        }

        public async Task<bool> EsAdminAsync(int userId, int ligaId)
        {
            return await _context.LigaMiembros
                .AnyAsync(lm => lm.UserId == userId && lm.LigaId == ligaId && lm.EsAdmin);
        }
    }
}
