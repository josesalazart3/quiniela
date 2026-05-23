// Ranking global individual cruzando todas las ligas de apuesta
namespace Quiniela.Models.DTOs
{
    public class RankingGlobalUsuarioReadDto
    {
        public int Posicion { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TotalPuntos { get; set; }
        public decimal? PremioAsignado { get; set; }
        public int? LigaId { get; set; }
        public string? NombreLiga { get; set; }
    }
}