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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(s => s.Direccion).IsRequired().HasMaxLength(250);
                entity.Property(s => s.Ciudad).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Turno>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Cedula)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(t => t.Estado)
                    .HasConversion<string>()   // guarda el enum como texto legible, no como número
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(t => t.FechaCreacionUtc).IsRequired();
                entity.Property(t => t.FechaExpiracionUtc).IsRequired();

                // Relación: un Turno pertenece a una Sucursal (obligatoria)
                entity.HasOne(t => t.Sucursal)
                    .WithMany(s => s.Turnos)
                    .HasForeignKey(t => t.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict); // no borrar turnos si se borra la sucursal

                // Índice compuesto: acelera la consulta "cuántos turnos tiene esta cédula hoy"
                entity.HasIndex(t => new { t.Cedula, t.FechaCreacionUtc });
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.NombreUsuario).IsUnique(); // no puede haber dos usuarios con el mismo nombre
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Rol).IsRequired().HasMaxLength(50);
            });
        }
    }
}

