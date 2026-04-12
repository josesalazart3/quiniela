// Solo se puede crear si faltan más de 15 minutos para el partido — validación en el servicio
namespace Quiniela.Models.DTOs
{
    public class PrediccionCreateDto
    {
        public int PartidoId { get; set; }
        public int LigaId { get; set; }
        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }
    }
}