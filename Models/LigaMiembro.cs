using Quiniela.Enums;
namespace Quiniela.Models
{
    public class LigaMiembro
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int LigaId { get; set; }
        public Liga? Liga { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;

        public bool EsAdmin { get; set; } = false;

        public int Puntos { get; set; } = 0;

        public DateTime FechaUnion { get; set; } = DateTime.UtcNow;

        public EstadoMiembro Estado { get; set; } = EstadoMiembro.Pendiente;

    }


}