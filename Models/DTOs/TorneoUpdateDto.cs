namespace Quiniela.Models.DTOs
{
    public class TorneoUpdateDto
    {
        public string? Nombre { get; set; }
        public string? PaisSede { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; } //A
    }
}