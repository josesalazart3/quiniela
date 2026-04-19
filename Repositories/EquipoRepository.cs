using Quiniela.Models;
using Quiniela.Data;
using Quiniela.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quiniela.Repositories
{
    public class EquipoRepository(AppDbContext context) : IEquipoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Equipo> AddEquipoAsync(Equipo equipo)
        {
            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();
            return equipo;
        }

        public async Task<IEnumerable<Equipo>> GetAllEquiposAsync(int page, int pageSize)
        {
            return await _context.Equipos
                .AsNoTracking()
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Equipo>> GetEquiposSelectAsync()
        {
            return await _context.Equipos
                .AsNoTracking()
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<Equipo?> GetEquipoByIdAsync(int id)
        {
            return await _context.Equipos.FindAsync(id);
        }

        public async Task<Equipo?> UpdateEquipoAsync(Equipo equipo)
        {
            var existing = await _context.Equipos.FindAsync(equipo.Id);
            if (existing == null) return null;

            existing.Nombre = equipo.Nombre;
            existing.CodigoFifa = equipo.CodigoFifa;
            existing.BanderaUrl = equipo.BanderaUrl;
            existing.Entrenador = equipo.Entrenador;
            existing.Capitan = equipo.Capitan;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteEquipoAsync(int id)
        {
            var entity = await _context.Equipos.FindAsync(id);
            if (entity == null) return false;

            _context.Equipos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}