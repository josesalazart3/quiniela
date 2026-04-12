// Usuario solicita unirse a una liga buscándola por nombre
namespace Quiniela.Models.DTOs
{
    public class LigaMiembroCreateDto
    {
        public required string NombreEquipo { get; set; }
    }
}