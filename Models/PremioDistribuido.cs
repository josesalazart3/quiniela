namespace Quiniela.Models
{
    public class PremioDistribuido
    {
        public int Id { get; set; }

        public int TorneoId { get; set; }
        public Torneo Torneo { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? LigaId { get; set; }
        public Liga? Liga { get; set; }

        public string Concepto { get; set; } = string.Empty; // "1er lugar Liga X", "Top 3 Global", etc.
        public decimal Monto { get; set; }
        public int Posicion { get; set; }
        public DateTime FechaDistribucion { get; set; } = DateTime.UtcNow;
    }
}