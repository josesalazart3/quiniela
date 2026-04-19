using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class EstadioRepository(AppDbContext context) : IEstadioRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Estadio> AddEstadioAsync(Estadio estadio)
        {
            _context.Estadios.Add(estadio);
            await _context.SaveChangesAsync();
            return estadio;
        }

        public async Task<IEnumerable<Estadio>> GetAllEstadiosAsync(int page, int pageSize)
        {
            return await _context.Estadios
                .AsNoTracking()
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Estadio>> GetEstadiosSelectAsync()
        {
            return await _context.Estadios
                .AsNoTracking()
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<Estadio?> GetEstadioByIdAsync(int id)
        {
            return await _context.Estadios.FindAsync(id);
        }

        public async Task<Estadio?> UpdateEstadioAsync(Estadio estadio)
        {
            var existing = await _context.Estadios.FindAsync(estadio.Id);
            if (existing == null) return null;

            existing.Nombre = estadio.Nombre;
            existing.Ciudad = estadio.Ciudad;
            existing.Pais = estadio.Pais;
            existing.Capacidad = estadio.Capacidad;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteEstadioAsync(int id)
        {
            var entity = await _context.Estadios.FindAsync(id);
            if (entity == null) return false;

            _context.Estadios.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}