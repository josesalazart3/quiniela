namespace Quiniela.Models.DTOs
{
    public class EquipoCreateDto
    {
        public required string Nombre { get; set; }
        public string CodigoFifa { get; set; } = string.Empty;
        public string BanderaUrl { get; set; } = string.Empty;
        public string Entrenador { get; set; } = string.Empty;
        public string Capitan { get; set; } = string.Empty;
    }
}