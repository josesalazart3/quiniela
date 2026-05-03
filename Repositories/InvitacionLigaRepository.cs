using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class InvitacionLigaRepository(AppDbContext context) : IInvitacionLigaRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<InvitacionLiga> AddInvitacionAsync(InvitacionLiga invitacion)
        {
            _context.InvitacionesLiga.Add(invitacion);
            await _context.SaveChangesAsync();
            return invitacion;
        }

        public async Task<IEnumerable<InvitacionLiga>> GetInvitacionesByLigaAsync(int ligaId)
        {
            return await _context.InvitacionesLiga
                .AsNoTracking()
                .Include(i => i.User)
                .Where(i => i.LigaId == ligaId)
                .OrderByDescending(i => i.FechaEnvio)
                .ToListAsync();
        }

        public async Task<InvitacionLiga?> GetInvitacionByIdAsync(int id)
        {
            return await _context.InvitacionesLiga
                .Include(i => i.Liga)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<InvitacionLiga?> GetInvitacionByTokenAsync(string token)
        {
            return await _context.InvitacionesLiga
                .Include(i => i.Liga)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task<InvitacionLiga?> GetInvitacionByEmailYLigaAsync(string email, int ligaId)
        {
            return await _context.InvitacionesLiga
                .FirstOrDefaultAsync(i => i.EmailInvitado == email && i.LigaId == ligaId);
        }

        public async Task<InvitacionLiga?> UpdateInvitacionAsync(InvitacionLiga invitacion)
        {
            var existing = await _context.InvitacionesLiga.FindAsync(invitacion.Id);
            if (existing == null) return null;

            existing.Estado = invitacion.Estado;
            existing.UserId = invitacion.UserId;
            existing.FechaRespuesta = invitacion.FechaRespuesta;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteInvitacionAsync(int id)
        {
            var entity = await _context.InvitacionesLiga.FindAsync(id);
            if (entity == null) return false;

            _context.InvitacionesLiga.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}