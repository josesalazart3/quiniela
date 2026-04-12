namespace Quiniela.Models.DTOs
{
    public class TorneoReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Año { get; set; }
        public string PaisSede { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}