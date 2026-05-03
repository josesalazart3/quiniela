using Microsoft.AspNetCore.SignalR;

namespace Quiniela.Hubs
{
    public class QuinielaHub : Hub
    {
        public async Task UnirseATorneo(int torneoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"torneo_{torneoId}");
        }

        public async Task UnirseALiga(int ligaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"liga_{ligaId}");
        }

        public async Task SalirDeTorneo(int torneoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"torneo_{torneoId}");
        }

        public async Task SalirDeLiga(int ligaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"liga_{ligaId}");
        }
    }
}