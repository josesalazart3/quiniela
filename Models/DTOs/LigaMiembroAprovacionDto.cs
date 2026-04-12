// Admin aprueba o rechaza — dispara automáticamente el estado en LigaMiembro
namespace Quiniela.Models.DTOs
{
    public class LigaMiembroAprobacionDto
    {
        public int UserId { get; set; }
        public bool Aprobar { get; set; }
    }
}