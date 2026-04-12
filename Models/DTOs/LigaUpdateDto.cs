namespace Quiniela.Models.DTOs
{
    public class LigaUpdateDto
    {
        public string? Nombre { get; set; }
        public bool? EsDeApuestas { get; set; }
        public decimal? PrecioPorUnirse { get; set; }
    }
}