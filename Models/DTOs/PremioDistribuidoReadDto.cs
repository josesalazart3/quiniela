namespace Quiniela.Models.DTOs
{
    public class PremioDistribuidoReadDto
    {
        public int Id { get; set; }
        public int TorneoId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? LigaId { get; set; }
        public string? NombreLiga { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public int Posicion { get; set; }
        public DateTime FechaDistribucion { get; set; }
    }
}