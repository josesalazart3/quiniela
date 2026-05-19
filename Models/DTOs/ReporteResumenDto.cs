namespace Quiniela.Models.DTOs
{
    public class ReporteResumenDto
    {
        public int TotalUsuarios { get; set; }
        public int TotalLigas { get; set; }
        public int TotalLigasApuesta { get; set; }
        public int TotalLigasDiversion { get; set; }
        public int TotalPredicciones { get; set; }
        public int PartidosJugados { get; set; }
        public int PartidosPendientes { get; set; }
        public int InvitacionesPendientes { get; set; }
        public decimal TotalRecaudado { get; set; }
        public int TotalSesionesActivas { get; set; }
    }
}