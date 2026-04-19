using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class GrupoRepository(AppDbContext context) : IGrupoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Grupo> AddGrupoAsync(Grupo grupo)
        {
            _context.Grupos.Add(grupo);
            await _context.SaveChangesAsync();
            return grupo;
        }

        public async Task<IEnumerable<Grupo>> GetAllGruposAsync(int page, int pageSize)
        {
            return await _context.Grupos
                .AsNoTracking()
                .Include(g => g.Torneo)
                .Include(g => g.Equipos)
                    .ThenInclude(ge => ge.Equipo)
                .OrderBy(g => g.TorneoId)
                .ThenBy(g => g.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Grupo>> GetGruposByTorneoAsync(int torneoId)
        {
            return await _context.Grupos
                .AsNoTracking()
                .Include(g => g.Torneo)
                .Include(g => g.Equipos)
                    .ThenInclude(ge => ge.Equipo)
                .Where(g => g.TorneoId == torneoId)
                .OrderBy(g => g.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Grupo>> GetGruposSelectAsync(int torneoId)
        {
            return await _context.Grupos
                .AsNoTracking()
                .Where(g => g.TorneoId == torneoId)
                .OrderBy(g => g.Nombre)
                .ToListAsync();
        }

        public async Task<Grupo?> GetGrupoByIdAsync(int id)
        {
            return await _context.Grupos.FindAsync(id);
        }

        public async Task<Grupo?> GetGrupoByIdWithDetailsAsync(int id)
        {
            return await _context.Grupos
                .Include(g => g.Torneo)
                .Include(g => g.Equipos)
                    .ThenInclude(ge => ge.Equipo)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Grupo?> UpdateGrupoAsync(Grupo grupo)
        {
            var existing = await _context.Grupos.FindAsync(grupo.Id);
            if (existing == null) return null;

            existing.Nombre = grupo.Nombre;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteGrupoAsync(int id)
        {
            var entity = await _context.Grupos.FindAsync(id);
            if (entity == null) return false;

            _context.Grupos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EquipoYaEnGrupoAsync(int grupoId, int equipoId)
        {
            return await _context.GrupoEquipos
                .AnyAsync(ge => ge.GrupoId == grupoId && ge.EquipoId == equipoId);
        }

        public async Task AddEquipoAGrupoAsync(GrupoEquipo grupoEquipo)
        {
            _context.GrupoEquipos.Add(grupoEquipo);
            await _context.SaveChangesAsync();
        }
        public async Task AsignarVariosEquiposAsync(List<GrupoEquipo> grupoEquipos)
        {
            await _context.GrupoEquipos.AddRangeAsync(grupoEquipos);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveEquipoDeGrupoAsync(int grupoId, int equipoId)
        {
            var entity = await _context.GrupoEquipos
                .FirstOrDefaultAsync(ge => ge.GrupoId == grupoId && ge.EquipoId == equipoId);
            if (entity == null) return false;

            _context.GrupoEquipos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}