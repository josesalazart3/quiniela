// Ranking dentro de una liga — incluye premio si es de apuestas
namespace Quiniela.Models.DTOs
{
    public class RankingLigaReadDto
    {
        public int Posicion { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public int Puntos { get; set; }
        public decimal? PremioAsignado { get; set; }
    }
}