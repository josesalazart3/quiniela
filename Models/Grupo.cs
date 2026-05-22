namespace Quiniela.Models
{
    public class Grupo
    {
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public int TorneoId { get; set; }

        public Torneo Torneo { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<GrupoEquipo> Equipos { get; set; } = new List<GrupoEquipo>();

        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}