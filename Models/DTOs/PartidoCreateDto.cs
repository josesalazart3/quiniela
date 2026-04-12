namespace Quiniela.Models.DTOs
{
    public class PartidoCreateDto
    {
        public int TorneoId { get; set; }
        public int FaseId { get; set; }
        public int? GrupoId { get; set; }
        public int? EquipoLocalId { get; set; }
        public int? EquipoVisitanteId { get; set; }
        public string? DescripcionLocal { get; set; }
        public string? DescripcionVisitante { get; set; }
        public DateTime FechaHora { get; set; }
        public int EstadioId { get; set; }
    }
}