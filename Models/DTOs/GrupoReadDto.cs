namespace Quiniela.Models.DTOs
{
    public class GrupoReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TorneoId { get; set; }
        public string TorneoNombre { get; set; } = string.Empty;
        public List<EquipoReadDto> Equipos { get; set; } = new();
    }
}