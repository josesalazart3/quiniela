using Quiniela.Models;

namespace Quiniela.Repositories.Interfaces
{
    public interface IInvitacionLigaRepository
    {
        Task<InvitacionLiga> AddInvitacionAsync(InvitacionLiga invitacion);
        Task<IEnumerable<InvitacionLiga>> GetInvitacionesByLigaAsync(int ligaId);
        Task<InvitacionLiga?> GetInvitacionByIdAsync(int id);
        Task<InvitacionLiga?> GetInvitacionByTokenAsync(string token);
        Task<InvitacionLiga?> GetInvitacionByEmailYLigaAsync(string email, int ligaId);
        Task<InvitacionLiga?> UpdateInvitacionAsync(InvitacionLiga invitacion);
        Task<bool> DeleteInvitacionAsync(int id);
    }
}