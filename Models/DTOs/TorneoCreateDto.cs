namespace Quiniela.Models.DTOs
{
    public class TorneoCreateDto
    {
        public required string Nombre { get; set; }
        public int Año { get; set; }
        public string PaisSede { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}