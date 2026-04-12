// El invitado responde usando el token del link
// Si no estaba registrado y acepta, se registra primero y luego queda Pendiente de aprobación
namespace Quiniela.Models.DTOs
{
    public class InvitacionResponderDto
    {
        public required string Token { get; set; }
        public bool Aceptar { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
    }
}