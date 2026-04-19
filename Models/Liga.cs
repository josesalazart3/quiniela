namespace Quiniela.Models
{
    public class Liga
    {
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public bool EsDeApuestas { get; set; }

        public decimal? PrecioPorUnirse { get; set; }

        public int TorneoId { get; set; }
        public Torneo? Torneo { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<LigaMiembro> LigaMiembros { get; set; } = new List<LigaMiembro>();
        public ICollection<Prediccion> Predicciones { get; set; } = new List<Prediccion>();

        public ICollection<InvitacionLiga> Invitaciones { get; set; } = new List<InvitacionLiga>();

        public DateTime? DeletedAt { get; set; }


    }
}