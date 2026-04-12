namespace Quiniela.Models.DTOs
{
    public class EstadioCreateDto
    {
        public required string Nombre { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public int Capacidad { get; set; }
    }
}