using Microsoft.EntityFrameworkCore;
using Quiniela.Models;

namespace Quiniela.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Liga> Ligas => Set<Liga>();
        public DbSet<LigaMiembro> LigaMiembros => Set<LigaMiembro>();
        public DbSet<Torneo> Torneos => Set<Torneo>();
        public DbSet<Equipo> Equipos => Set<Equipo>();
        public DbSet<Grupo> Grupos => Set<Grupo>();
        public DbSet<GrupoEquipo> GrupoEquipos => Set<GrupoEquipo>();
        public DbSet<Fase> Fases => Set<Fase>();
        public DbSet<Partido> Partidos => Set<Partido>();
        public DbSet<Prediccion> Predicciones => Set<Prediccion>();
        public DbSet<Estadio> Estadios => Set<Estadio>();
        public DbSet<ClasificacionGrupo> ClasificacionGrupos => Set<ClasificacionGrupo>();
        public DbSet<InvitacionLiga> InvitacionesLiga => Set<InvitacionLiga>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();



        private const string AdminPasswordHash = "$2a$11$D13huptFPV/i1px.II67.uvGztGXJfYqusE2hahkgCFJ0R3oPVVre";

        private static readonly DateTime SeedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);
            modelBuilder.Entity<Liga>().HasQueryFilter(l => l.DeletedAt == null);
            modelBuilder.Entity<Prediccion>().HasQueryFilter(p => p.DeletedAt == null);
            modelBuilder.Entity<LigaMiembro>().HasQueryFilter(lm => lm.DeletedAt == null);
            modelBuilder.Entity<InvitacionLiga>().HasQueryFilter(i => i.DeletedAt == null);
            modelBuilder.Entity<Partido>().HasQueryFilter(p => p.DeletedAt == null);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Liga>()
                .HasOne(l => l.CreatedByUser)
                .WithMany(u => u.LigasCreadas)
                .HasForeignKey(l => l.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Liga>()
                .HasOne(l => l.Torneo)
                .WithMany(t => t.Ligas)
                .HasForeignKey(l => l.TorneoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LigaMiembro>()
                .HasKey(lm => new { lm.UserId, lm.LigaId });

            modelBuilder.Entity<LigaMiembro>()
                .HasOne(lm => lm.User)
                .WithMany(u => u.LigaMiembros)
                .HasForeignKey(lm => lm.UserId);

            modelBuilder.Entity<LigaMiembro>()
                .HasOne(lm => lm.Liga)
                .WithMany(l => l.LigaMiembros)
                .HasForeignKey(lm => lm.LigaId);

            modelBuilder.Entity<InvitacionLiga>()
                .HasOne(i => i.Liga)
                .WithMany(l => l.Invitaciones)
                .HasForeignKey(i => i.LigaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvitacionLiga>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invitaciones)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            ///token para invitar
            modelBuilder.Entity<InvitacionLiga>()
                .HasIndex(i => i.Token)
                .IsUnique();

            modelBuilder.Entity<Grupo>()
                .HasOne(g => g.Torneo)
                .WithMany(t => t.Grupos)
                .HasForeignKey(g => g.TorneoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Fase>()
                .HasOne(f => f.Torneo)
                .WithMany(t => t.Fases)
                .HasForeignKey(f => f.TorneoId)
                .OnDelete(DeleteBehavior.Cascade);

            // GRUPO EQUIPO
            modelBuilder.Entity<GrupoEquipo>()
                .HasKey(ge => new { ge.GrupoId, ge.EquipoId });

            modelBuilder.Entity<GrupoEquipo>()
                .HasOne(ge => ge.Grupo)
                .WithMany(g => g.Equipos)
                .HasForeignKey(ge => ge.GrupoId);

            modelBuilder.Entity<GrupoEquipo>()
                .HasOne(ge => ge.Equipo)
                .WithMany(e => e.Grupos)
                .HasForeignKey(ge => ge.EquipoId);

            modelBuilder.Entity<ClasificacionGrupo>()
                .HasOne(c => c.Grupo)
                .WithMany()
                .HasForeignKey(c => c.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClasificacionGrupo>()
                .HasOne(c => c.Equipo)
                .WithMany()
                .HasForeignKey(c => c.EquipoId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ClasificacionGrupo>()
                .HasIndex(c => new { c.GrupoId, c.EquipoId })
                .IsUnique();


            // PARTIDO
            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Torneo)
                .WithMany(t => t.Partidos)
                .HasForeignKey(p => p.TorneoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Fase)
                .WithMany(f => f.Partidos)
                .HasForeignKey(p => p.FaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Grupo)
                .WithMany(g => g.Partidos)
                .HasForeignKey(p => p.GrupoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.EquipoLocal)
                .WithMany()
                .HasForeignKey(p => p.EquipoLocalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.EquipoVisitante)
                .WithMany()
                .HasForeignKey(p => p.EquipoVisitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Estadio)
                .WithMany(e => e.Partidos)
                .HasForeignKey(p => p.EstadioId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Prediccion>()
                .HasOne(p => p.User)
                .WithMany(u => u.Predicciones)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prediccion>()
                .HasOne(p => p.Liga)
                .WithMany(l => l.Predicciones)
                .HasForeignKey(p => p.LigaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prediccion>()
                .HasOne(p => p.Partido)
                .WithMany(pa => pa.Predicciones)
                .HasForeignKey(p => p.PartidoId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Prediccion>()
                .HasIndex(p => new { p.UserId, p.LigaId, p.PartidoId })
                .IsUnique();


            // SEED DATA
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Administrador", CreatedAt = SeedDate },
                new Role { Id = 2, Name = "User", CreatedAt = SeedDate }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Password = AdminPasswordHash,
                    Email = "admin@quiniela.com",
                    FirstName = "System",
                    LastName = "Admin",
                    RoleId = 1,
                    CreatedAt = SeedDate
                }
            );

            modelBuilder.Entity<UserSession>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sesiones)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("audit_logs");
                entity.Property(e => e.Tabla).HasColumnName("tabla");
                entity.Property(e => e.Operacion).HasColumnName("operacion");
                entity.Property(e => e.ValorAnterior)
                    .HasColumnName("valor_anterior")
                    .HasColumnType("jsonb");
                entity.Property(e => e.ValorNuevo)
                    .HasColumnName("valor_nuevo")
                    .HasColumnType("jsonb");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.FechaHora).HasColumnName("fecha_hora");
            });
        }
    }
}