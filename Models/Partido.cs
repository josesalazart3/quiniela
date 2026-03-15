namespace Quiniela.Models
{
    public class Partido
    {
        public int Id { get; set; }

        public int TorneoId { get; set; }

        public Torneo Torneo { get; set; } = null!;

        public int FaseId { get; set; }

        public Fase Fase { get; set; } = null!;

        public int? GrupoId { get; set; }

        public Grupo? Grupo { get; set; }

        public int? EquipoLocalId { get; set; }

        public Equipo? EquipoLocal { get; set; }

        public int? EquipoVisitanteId { get; set; }

        public Equipo? EquipoVisitante { get; set; }

        public DateTime FechaHora { get; set; }

        public int EstadioId { get; set; }

        public Estadio Estadio { get; set; } = null!;

        public int? GolesLocal { get; set; }

        public int? GolesVisitante { get; set; }
        public string? DescripcionLocal { get; set; }
        public string? DescripcionVisitante { get; set; }

        public bool Finalizado { get; set; } = false;

        public ICollection<Prediccion> Predicciones { get; set; } = new List<Prediccion>();
    }
}