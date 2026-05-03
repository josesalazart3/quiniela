using Quiniela.Models.DTOs;

namespace Quiniela.Services.Interfaces
{
    public interface IInvitacionLigaService
    {
        Task<InvitacionReadDto> EnviarInvitacionAsync(int ligaId, InvitacionCreateDto dto, int adminId);
        Task<IEnumerable<InvitacionReadDto>> GetInvitacionesByLigaAsync(int ligaId, int adminId);
        Task<InvitacionReadDto?> ResponderInvitacionAsync(InvitacionResponderDto dto, int? userId);
        Task<bool> CancelarInvitacionAsync(int invitacionId, int adminId);
    }
}