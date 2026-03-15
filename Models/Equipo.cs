namespace Quiniela.Models
{
    public class Equipo
    {
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public string CodigoFifa { get; set; } = string.Empty;

        public string BanderaUrl { get; set; } = string.Empty;

        public ICollection<GrupoEquipo> Grupos { get; set; } = new List<GrupoEquipo>();
    }
}