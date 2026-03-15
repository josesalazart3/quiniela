namespace Quiniela.Models
{
    public class Fase
    {
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public int Orden { get; set; }

        public int TorneoId { get; set; }

        public Torneo? Torneo { get; set; }

        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}