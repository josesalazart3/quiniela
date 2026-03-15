using Quiniela.Enums;
namespace Quiniela.Models
{
    public class InvitacionLiga
    {
        public int Id { get; set; }

        public int LigaId { get; set; }
        public Liga Liga { get; set; } = null!;

        public string EmailInvitado { get; set; } = string.Empty;

        public int? UserId { get; set; }
        public User? User { get; set; }

        public string Token { get; set; } = string.Empty; // GUID para el link de invitación

        public EstadoInvitacion Estado { get; set; } = EstadoInvitacion.Pendiente;

        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaExpiracion { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }

}