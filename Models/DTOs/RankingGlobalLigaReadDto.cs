// Liga con mayor promedio de puntos — para el premio global de liga
namespace Quiniela.Models.DTOs
{
    public class RankingGlobalLigaReadDto
    {
        public int Posicion { get; set; }
        public int LigaId { get; set; }
        public string NombreLiga { get; set; } = string.Empty;
        public double PromedioPuntos { get; set; }
        public int TotalMiembros { get; set; }
        public decimal? PremioTotal { get; set; }
        public decimal? PremioPerCapita { get; set; }
    }
}