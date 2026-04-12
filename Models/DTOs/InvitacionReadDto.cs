namespace Quiniela.Models.DTOs
{
    public class InvitacionReadDto
    {
        public int Id { get; set; }
        public string EmailInvitado { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }
}