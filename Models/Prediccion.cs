namespace Quiniela.Models
{
    public class Prediccion
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int LigaId { get; set; }

        public Liga Liga { get; set; } = null!;

        public int PartidoId { get; set; }

        public Partido Partido { get; set; } = null!;

        public int GolesLocal { get; set; }

        public int GolesVisitante { get; set; }

        public int PuntosGanados { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}