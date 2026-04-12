// Solo se puede actualizar si faltan más de 15 minutos para el partido — validación en el servicio
namespace Quiniela.Models.DTOs
{
    public class PrediccionUpdateDto
    {
        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }
    }
}