namespace Quiniela.Models.DTOs
{
    public class LigaReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EsDeApuestas { get; set; }
        public decimal? PrecioPorUnirse { get; set; }
        public int TorneoId { get; set; }
        public string CreadaPor { get; set; } = string.Empty;
        public decimal? FondoTotal { get; set; }

        public int TotalMiembros { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}