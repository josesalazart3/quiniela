namespace Quiniela.Models.DTOs
{
    public class FaseCreateDto
    {
        public required string Nombre { get; set; }
        public int Orden { get; set; }
        public int TorneoId { get; set; }
    }
}