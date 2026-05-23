namespace Quiniela.Models.DTOs
{
    public class MejoresTercerosDto
    {
        public int GrupoId { get; set; }
        public string GrupoNombre { get; set; } = string.Empty;
        public int EquipoId { get; set; }
        public string EquipoNombre { get; set; } = string.Empty;
        public string BanderaUrl { get; set; } = string.Empty;
        public int Puntos { get; set; }
        public int DiferenciaGoles { get; set; }
        public int GolesAFavor { get; set; }
        public int PartidosJugados { get; set; }
        public int Posicion { get; set; } // siempre 3
    }
}