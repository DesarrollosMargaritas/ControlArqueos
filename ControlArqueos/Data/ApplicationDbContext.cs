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

        // --- TABLAS DEL ERP (LASMARGARITAS) ---
        public DbSet<RemFront> PuntosDeVenta { get; set; }
        public DbSet<RemCajaFront> Cajas { get; set; }
        public DbSet<Arqueo> Arqueos { get; set; }
        public DbSet<ArqueoLin> ArqueoLineas { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<TipoPago> TiposPago { get; set; }
        public DbSet<FacturaVenta> FacturasVenta { get; set; }

        // NUEVO
        public DbSet<CobroPago> CobrosPagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Puntos de Venta
            modelBuilder.Entity<RemFront>(entity =>
            {
                entity.ToTable("REM_FRONTS");
                entity.HasKey(e => e.IdFront);
                entity.HasMany(d => d.Cajas).WithOne().HasForeignKey(p => p.IdFront);
            });

            // Cajas (Llave Compuesta)
            modelBuilder.Entity<RemCajaFront>(entity =>
            {
                entity.ToTable("REM_CAJASFRONT");
                entity.HasKey(e => new { e.IdFront, e.CajaFront });
                entity.Property(e => e.CajaManager).HasColumnName("CAJAMANAGER");
            });

            // Arqueos (Cabecera)
            modelBuilder.Entity<Arqueo>(entity =>
            {
                entity.ToTable("ARQUEOS");
                entity.HasKey(e => new { e.Caja, e.Numero });
            });

            // Arqueos (Detalle)
            modelBuilder.Entity<ArqueoLin>(entity =>
            {
                entity.ToTable("ARQUEOSLIN");
                entity.HasKey(e => new { e.Caja, e.Numero, e.CodTipoPago });
            });

            // Facturas (Para Propinas)
            modelBuilder.Entity<FacturaVenta>(entity =>
            {
                entity.ToTable("FACTURASVENTA");
                entity.HasKey(e => new { e.NumSerie, e.NumFactura });
            });

            // NUEVO: CobrosPagos (Para Anticipos)
            modelBuilder.Entity<CobroPago>(entity =>
            {
                entity.ToTable("COBROSPAGOS");
                entity.HasNoKey(); // Marcamos sin llave si solo es para lectura agregada
            });

            // Tablas Auxiliares
            modelBuilder.Entity<Vendedor>().HasKey(e => e.CodVendedor);
            modelBuilder.Entity<TipoPago>().HasKey(e => e.CodTipoPago);

            base.OnModelCreating(modelBuilder);
        }
    }
}