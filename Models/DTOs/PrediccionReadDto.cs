namespace Quiniela.Models.DTOs
{
    public class PrediccionReadDto
    {
        public int Id { get; set; }
        public int PartidoId { get; set; }
        public int LigaId { get; set; }
        public string EquipoLocal { get; set; } = string.Empty;
        public string EquipoVisitante { get; set; } = string.Empty;
        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }
        public int PuntosGanados { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}