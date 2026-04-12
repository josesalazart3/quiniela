namespace Quiniela.Models.DTOs
{
    public class PremiosGlobalesReadDto
    {
        public decimal TotalRecaudadoGlobal { get; set; }
        public decimal MontoGlobalIndividual { get; set; }    // 0.5% del total
        public decimal MontoGlobalLiga { get; set; }          // 0.5% del total
        public List<RankingGlobalUsuarioReadDto> TopIndividuales { get; set; } = new();
        public RankingGlobalLigaReadDto? MejorLiga { get; set; }
    }
}