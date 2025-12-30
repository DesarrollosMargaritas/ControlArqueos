using Microsoft.EntityFrameworkCore;
using TesoreriaMargaritas.Models;

namespace TesoreriaMargaritas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RemFront> PuntosDeVenta { get; set; }
        public DbSet<RemCajaFront> Cajas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- SOLUCIÓN DEL ERROR ---
            // Forzamos explícitamente el nombre de las tablas SQL aquí.
            // Esto tiene prioridad sobre cualquier convención de nombres de C#.

            modelBuilder.Entity<RemFront>(entity =>
            {
                entity.ToTable("REM_FRONTS"); // Nombre real en SQL
                entity.HasKey(e => e.IdFront); // Llave primaria

                // Configuración de la relación 1 a muchos
                entity.HasMany(d => d.Cajas)
                      .WithOne()
                      .HasForeignKey(p => p.IdFront);
            });

            modelBuilder.Entity<RemCajaFront>(entity =>
            {
                entity.ToTable("REM_CAJASFRONT"); // Nombre real en SQL
                entity.HasKey(e => e.CajaFront); // Llave primaria
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}