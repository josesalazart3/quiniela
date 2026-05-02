namespace Quiniela.Models
{
    public class AuditLog
    {
        public long Id { get; set; }
        public string Tabla { get; set; } = string.Empty;
        public string Operacion { get; set; } = string.Empty; // INSERT, UPDATE, DELETE
        public string? ValorAnterior { get; set; } // JSON
        public string? ValorNuevo { get; set; }    // JSON
        public int? UserId { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    }
}