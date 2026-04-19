using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IEquipoRepository
    {
        Task<Equipo> AddEquipoAsync(Equipo equipo);
        Task<IEnumerable<Equipo>> GetAllEquiposAsync(int page, int pageSize);
        Task<IEnumerable<Equipo>> GetEquiposSelectAsync();
        Task<Equipo?> GetEquipoByIdAsync(int id);
        Task<Equipo?> UpdateEquipoAsync(Equipo equipo);
        Task<bool> DeleteEquipoAsync(int id);
    }
}