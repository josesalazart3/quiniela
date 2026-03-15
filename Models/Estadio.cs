namespace Quiniela.Models
{
    public class Estadio
    {
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public string Ciudad { get; set; } = string.Empty;

        public string Pais { get; set; } = string.Empty;

        public int Capacidad { get; set; }

        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}