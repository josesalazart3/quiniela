// Solo lectura — se actualiza automáticamente al ingresar resultado de partido
namespace Quiniela.Models.DTOs
{
    public class ClasificacionGrupoReadDto
    {
        public EquipoReadDto Equipo { get; set; } = null!;
        public int PartidosJugados { get; set; }
        public int Ganados { get; set; }
        public int Empatados { get; set; }
        public int Perdidos { get; set; }
        public int GolesAFavor { get; set; }
        public int GolesEnContra { get; set; }
        public int DiferenciaGoles { get; set; }
        public int Puntos { get; set; }
    }
}