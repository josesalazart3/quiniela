// Para actualizar equipos cuando se define el bracket eliminatorio
namespace Quiniela.Models.DTOs
{
    public class PartidoUpdateDto
    {
        public int? EquipoLocalId { get; set; }
        public int? EquipoVisitanteId { get; set; }
        public string? DescripcionLocal { get; set; }
        public string? DescripcionVisitante { get; set; }
        public DateTime? FechaHora { get; set; }
        public int? EstadioId { get; set; }
    }
}