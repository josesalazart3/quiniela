namespace Quiniela.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendInvitacionLigaAsync(string emailDestino, string nombreLiga, string token);
        Task SendAprobacionMiembroAsync(string emailDestino, string nombreLiga);
        Task SendRecuperacionPasswordAsync(string emailDestino, string token);
    }
}
