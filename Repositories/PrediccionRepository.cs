using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class PrediccionRepository(AppDbContext context) : IPrediccionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Prediccion> AddPrediccionAsync(Prediccion prediccion)
        {
            _context.Predicciones.Add(prediccion);
            await _context.SaveChangesAsync();
            return prediccion;
        }

        public async Task<IEnumerable<Prediccion>> GetPrediccionesByLigaAsync(int ligaId, int page, int pageSize)
        {
            return await _context.Predicciones
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa.EquipoLocal)
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa.EquipoVisitante)
                .Where(p => p.LigaId == ligaId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prediccion>> GetPrediccionesByUserYLigaAsync(int userId, int ligaId, int page, int pageSize)
        {
            return await _context.Predicciones
                .AsNoTracking()
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa.EquipoLocal)
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa.EquipoVisitante)
                .Where(p => p.UserId == userId && p.LigaId == ligaId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prediccion>> GetPrediccionesByPartidoAsync(int partidoId)
        {
            return await _context.Predicciones
                .Include(p => p.User)
                .Where(p => p.PartidoId == partidoId)
                .ToListAsync();
        }

        public async Task<Prediccion?> GetPrediccionByIdAsync(int id)
        {
            return await _context.Predicciones
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa.EquipoLocal)
                .Include(p => p.Partido)
                    .ThenInclude(pa => pa.EquipoVisitante)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prediccion?> GetPrediccionByUserLigaPartidoAsync(int userId, int ligaId, int partidoId)
        {
            return await _context.Predicciones
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LigaId == ligaId && p.PartidoId == partidoId);
        }

        public async Task<Prediccion?> UpdatePrediccionAsync(Prediccion prediccion)
        {
            var existing = await _context.Predicciones.FindAsync(prediccion.Id);
            if (existing == null) return null;

            existing.GolesLocal = prediccion.GolesLocal;
            existing.GolesVisitante = prediccion.GolesVisitante;
            existing.UpdatedAt = prediccion.UpdatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task ActualizarPuntosPrediccionesAsync(int partidoId, int golesLocal, int golesVisitante)
        {
            var predicciones = await _context.Predicciones
                .Where(p => p.PartidoId == partidoId)
                .ToListAsync();

            foreach (var prediccion in predicciones)
            {
                var puntosAnteriores = prediccion.PuntosGanados;
                int nuevosPuntos;

                if (prediccion.GolesLocal == golesLocal && prediccion.GolesVisitante == golesVisitante)
                {
                    nuevosPuntos = 3;
                }
                else if (
                    (prediccion.GolesLocal > prediccion.GolesVisitante && golesLocal > golesVisitante) ||
                    (prediccion.GolesLocal < prediccion.GolesVisitante && golesLocal < golesVisitante) ||
                    (prediccion.GolesLocal == prediccion.GolesVisitante && golesLocal == golesVisitante))
                {
                    nuevosPuntos = 1;
                }
                else
                {
                    nuevosPuntos = 0;
                }

                prediccion.PuntosGanados = nuevosPuntos;

                var miembro = await _context.LigaMiembros
                    .FirstOrDefaultAsync(lm => lm.UserId == prediccion.UserId && lm.LigaId == prediccion.LigaId);

                if (miembro != null)
                    miembro.Puntos += (nuevosPuntos - puntosAnteriores);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePrediccionAsync(int id)
	{
	    var prediccion = await _context.Predicciones.FindAsync(id);
	    if (prediccion == null) return false;

	    prediccion.DeletedAt = DateTime.UtcNow;
	    await _context.SaveChangesAsync();
	    return true;
	}
    }
}
