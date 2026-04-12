namespace Quiniela.Models.DTOs
{
    public class LigaMiembroReadDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public int Puntos { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaUnion { get; set; }
    }
}