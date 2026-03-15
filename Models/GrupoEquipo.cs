namespace Quiniela.Models
{
    public class GrupoEquipo
    {
        public int GrupoId { get; set; }

        public Grupo Grupo { get; set; } = null!;

        public int EquipoId { get; set; }

        public Equipo Equipo { get; set; } = null!;
    }
}