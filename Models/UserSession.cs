using Quiniela.Enums;

namespace Quiniela.Models
{
    public class UserSession
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string IpOrigen { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public EstadoSesion Estado { get; set; } = EstadoSesion.Activa;
    }
}