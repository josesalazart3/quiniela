namespace Quiniela.Models.DTOs
{
    public class PartidoReadDto
    {
        public int Id { get; set; }
        public int TorneoId { get; set; }
        public FaseReadDto Fase { get; set; } = null!;
        public int? GrupoId { get; set; }
        public string? GrupoNombre { get; set; }
        public EquipoReadDto? EquipoLocal { get; set; }
        public EquipoReadDto? EquipoVisitante { get; set; }
        public string? DescripcionLocal { get; set; }
        public string? DescripcionVisitante { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadioReadDto Estadio { get; set; } = null!;
        public int? GolesLocal { get; set; }
        public int? GolesVisitante { get; set; }
        public bool Finalizado { get; set; }
    }
}