using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class ClasificacionGrupoRepository(AppDbContext context) : IClasificacionGrupoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<ClasificacionGrupo> AddClasificacionAsync(ClasificacionGrupo clasificacion)
        {
            _context.ClasificacionGrupos.Add(clasificacion);
            await _context.SaveChangesAsync();
            return clasificacion;
        }

        public async Task<IEnumerable<ClasificacionGrupo>> GetClasificacionByGrupoAsync(int grupoId)
        {
            return await _context.ClasificacionGrupos
                .AsNoTracking()
                .Include(c => c.Equipo)
                .Where(c => c.GrupoId == grupoId)
                .OrderByDescending(c => c.Puntos)
                .ThenByDescending(c => c.DiferenciaGoles)
                .ThenByDescending(c => c.GolesAFavor)
                .ToListAsync();
        }

        public async Task<ClasificacionGrupo?> GetClasificacionByGrupoYEquipoAsync(int grupoId, int equipoId)
        {
            return await _context.ClasificacionGrupos
                .FirstOrDefaultAsync(c => c.GrupoId == grupoId && c.EquipoId == equipoId);
        }

        public async Task<ClasificacionGrupo?> UpdateClasificacionAsync(ClasificacionGrupo clasificacion)
        {
            var existing = await _context.ClasificacionGrupos
                .FirstOrDefaultAsync(c => c.GrupoId == clasificacion.GrupoId && c.EquipoId == clasificacion.EquipoId);
            if (existing == null) return null;

            existing.PartidosJugados = clasificacion.PartidosJugados;
            existing.Ganados = clasificacion.Ganados;
            existing.Empatados = clasificacion.Empatados;
            existing.Perdidos = clasificacion.Perdidos;
            existing.GolesAFavor = clasificacion.GolesAFavor;
            existing.GolesEnContra = clasificacion.GolesEnContra;
            existing.DiferenciaGoles = clasificacion.DiferenciaGoles;
            existing.Puntos = clasificacion.Puntos;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteClasificacionAsync(int grupoId, int equipoId)
        {
            var entity = await _context.ClasificacionGrupos
                .FirstOrDefaultAsync(c => c.GrupoId == grupoId && c.EquipoId == equipoId);
            if (entity == null) return false;

            _context.ClasificacionGrupos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}