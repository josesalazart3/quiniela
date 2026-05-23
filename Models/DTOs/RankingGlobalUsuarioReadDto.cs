namespace Quiniela.Models.DTOs
{
    public class RankingGlobalUsuarioReadDto
    {
        public int Posicion { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TotalPuntos { get; set; }

        // Liga donde obtuvo su mejor puntaje
        public int? LigaId { get; set; }
        public string? NombreLiga { get; set; }

        public decimal? PremioAsignado { get; set; }
    }
}