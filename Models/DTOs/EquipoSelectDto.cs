namespace Quiniela.Models.DTOs
{
    public class EquipoSelectDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string BanderaUrl { get; set; } = string.Empty;
        public string CodigoFifa { get; set; } = string.Empty;
    }
}