namespace Quiniela.Models.DTOs
{
    public class FaseReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Orden { get; set; }
        public int TorneoId { get; set; }
        public string TorneoNombre { get; set; } = string.Empty;
    }
}