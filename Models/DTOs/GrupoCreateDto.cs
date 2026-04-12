namespace Quiniela.Models.DTOs
{
    public class GrupoCreateDto
    {
        public required string Nombre { get; set; }
        public int TorneoId { get; set; }
    }
}