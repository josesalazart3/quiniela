namespace Quiniela.Models.DTOs
{
    public class EquipoReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoFifa { get; set; } = string.Empty;
        public string BanderaUrl { get; set; } = string.Empty;
        public string Entrenador { get; set; } = string.Empty;
        public string Capitan { get; set; } = string.Empty;
    }
}