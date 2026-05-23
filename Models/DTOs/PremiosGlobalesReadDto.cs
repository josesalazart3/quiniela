namespace Quiniela.Models.DTOs
{
    public class PremiosGlobalesReadDto
    {
        public decimal TotalRecaudadoGlobal { get; set; }
        public decimal MontoGlobalIndividual { get; set; }
        public decimal MontoGlobalLiga { get; set; }
        public List<RankingGlobalUsuarioReadDto> TopIndividuales { get; set; } = new();
        public RankingGlobalLigaReadDto? MejorLiga { get; set; }
    }
}