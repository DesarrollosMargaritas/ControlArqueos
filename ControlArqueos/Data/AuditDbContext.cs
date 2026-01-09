using Microsoft.EntityFrameworkCore;
using TesoreriaMargaritas.Models;

namespace TesoreriaMargaritas.Data
{
    // Contexto EXCLUSIVO para la base de datos TESORERIA_AUDIT
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options)
            : base(options)
        {
        }

        public DbSet<CompensacionArqueo> Compensaciones { get; set; }
        // NUEVO
        public DbSet<PagoCierre> PagosCierre { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompensacionArqueo>(entity =>
            {
                entity.ToTable("COMPENSACIONES_ARQUEO");
                entity.HasKey(e => e.Id);
            });

            // NUEVO
            modelBuilder.Entity<PagoCierre>(entity =>
            {
                entity.ToTable("PAGOS_CIERRE");
                entity.HasKey(e => e.Id);
            });
        }
    }
}