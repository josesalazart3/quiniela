// El admin ingresa el resultado — dispara automáticamente:
// 1. Actualización de ClasificacionGrupo si es fase de grupos
// 2. Cálculo de PuntosGanados en todas las Predicciones de ese partido
// 3. Actualización de Puntos en todos los LigaMiembro afectados
namespace Quiniela.Models.DTOs
{
    public class PartidoResultadoDto
    {
        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }
    }
}