namespace Quiniela.Models
{
    public class ClasificacionGrupo
    {
        public int Id { get; set; }

        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; } = null!;

        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; } = null!;

        public int PartidosJugados { get; set; } = 0;
        public int Ganados { get; set; } = 0;
        public int Empatados { get; set; } = 0;
        public int Perdidos { get; set; } = 0;
        public int GolesAFavor { get; set; } = 0;
        public int GolesEnContra { get; set; } = 0;
        public int DiferenciaGoles { get; set; } = 0;
        public int Puntos { get; set; } = 0;
    }
}