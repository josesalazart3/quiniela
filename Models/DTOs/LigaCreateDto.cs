namespace Quiniela.Models.DTOs
{
    public class LigaCreateDto
    {
        public required string Nombre { get; set; }
        public bool EsDeApuestas { get; set; }
        public decimal? PrecioPorUnirse { get; set; }
        public int TorneoId { get; set; }
        // Al crear la liga el usuario queda como admin — necesita su NombreEquipo
        public required string NombreEquipo { get; set; }
    }
}