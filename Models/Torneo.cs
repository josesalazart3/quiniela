namespace Quiniela.Models
{
    public class Torneo
    {
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public int Año { get; set; }
        public string PaisSede { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public bool Finalizado { get; set; } = false;

        public ICollection<Liga> Ligas { get; set; } = new List<Liga>();

        public ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();
        public ICollection<Fase> Fases { get; set; } = new List<Fase>();

        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}