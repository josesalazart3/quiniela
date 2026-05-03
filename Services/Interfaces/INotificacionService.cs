namespace Quiniela.Services.Interfaces
{
    public interface INotificacionService
    {
        Task NotificarResultadoPartidoAsync(int torneoId, int partidoId);
        Task NotificarRankingLigaAsync(int ligaId);
    }
}