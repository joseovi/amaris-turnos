using AmarisTurnos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmarisTurnos.Infrastructure.Persistence
{
    public class TurnosDbContext : DbContext
    {
        public TurnosDbContext(DbContextOptions<TurnosDbContext> options)
            : base(options)
        {
        }

        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            // Fuerza que todo DateTime leído de la BD se trate como UTC,
            // evitando que EF Core lo devuelva con Kind=Unspecified.
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<UtcDateTimeConverter>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(s => s.Direccion).IsRequired().HasMaxLength(250);
                entity.Property(s => s.Ciudad).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Activo).IsRequired();
            });

            modelBuilder.Entity<Turno>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(t => t.FechaCreacionUtc).IsRequired();
                entity.Property(t => t.FechaExpiracionUtc).IsRequired();

                // Relación: un Turno pertenece a una Sucursal (obligatoria)
                entity.HasOne(t => t.Sucursal)
                    .WithMany(s => s.Turnos)
                    .HasForeignKey(t => t.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict); // no borrar turnos si se borra la sucursal

                entity.HasOne(t => t.Usuario)
                     .WithMany(u => u.Turnos)
                     .HasForeignKey(t => t.UsuarioId)
                     .OnDelete(DeleteBehavior.Restrict);

                // Índice compuesto: acelera la consulta "cuántos turnos tiene esta cédula hoy"
                entity.HasIndex(t => new { t.UsuarioId, t.FechaCreacionUtc });
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Cedula).IsRequired().HasMaxLength(20);
                entity.HasIndex(u => u.Cedula).IsUnique(); // no puede haber dos usuarios con el mismo nombre
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Email).HasMaxLength(150);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Rol).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(u => u.Activo).IsRequired();
                entity.Property(u => u.CreatedAtUtc).IsRequired();
            });
        }
    }
}

