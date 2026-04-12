// Para asignar un equipo a un grupo — también crea su ClasificacionGrupo automáticamente
namespace Quiniela.Models.DTOs
{
    public class GrupoEquipoCreateDto
    {
        public int GrupoId { get; set; }
        public int EquipoId { get; set; }
    }
}