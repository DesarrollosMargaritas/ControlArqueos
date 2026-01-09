using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesoreriaMargaritas.Models
{
    // --- TABLAS DE LA BASE DE DATOS (ENTIDADES) ---

    [Table("REM_FRONTS")]
    public class RemFront
    {
        [Key]
        public int IdFront { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public bool Descatalogado { get; set; }
        public List<RemCajaFront> Cajas { get; set; } = new List<RemCajaFront>();
    }

    [Table("REM_CAJASFRONT")]
    public class RemCajaFront
    {
        [Key]
        public int CajaFront { get; set; }
        public int IdFront { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string? CajaManager { get; set; }
    }

    [Table("ARQUEOS")]
    public class Arqueo
    {
        public string Caja { get; set; } = string.Empty;
        public double Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string? Hora { get; set; }

        [Column("CODVENDEDOR")]
        public int CodVendedor { get; set; }

        [ForeignKey("CodVendedor")]
        public Vendedor? Vendedor { get; set; }
    }

    [Table("ARQUEOSLIN")]
    public class ArqueoLin
    {
        public string Caja { get; set; } = string.Empty;
        public int Numero { get; set; }

        [Column("CODTIPOPAGO")]
        public string CodTipoPago { get; set; } = string.Empty;

        public double Ventas { get; set; }
        public double Abonos { get; set; }
        public double Fianza { get; set; }
        public double Pagos { get; set; }
        public double Anticipos { get; set; }
        public double Calculado { get; set; }
        public double Declarado { get; set; }
        public double Descuadre { get; set; }
        public double Retirado { get; set; }

        [ForeignKey("CodTipoPago")]
        public TipoPago? TipoPago { get; set; }
    }

    [Table("FACTURASVENTA")]
    public class FacturaVenta
    {
        [Column("NUMSERIE")]
        public string NumSerie { get; set; } = string.Empty;

        [Column("NUMFACTURA")]
        public int NumFactura { get; set; }

        [Column("CAJA")]
        public string Caja { get; set; } = string.Empty;

        [Column("Z")]
        public int Z { get; set; }

        [Column("PROPINA")]
        public double Propina { get; set; }
    }

    [Table("COMPENSACIONES_ARQUEO")]
    public class CompensacionArqueo
    {
        [Key]
        public int Id { get; set; }

        public string Caja { get; set; } = string.Empty;
        public double NumeroArqueo { get; set; }

        public double Valor { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string Usuario { get; set; } = "Sistema";
    }

    // NUEVO: Tabla para control de gastos posteriores (afecta al Asegurado)
    [Table("PAGOS_CIERRE")]
    public class PagoCierre
    {
        [Key]
        public int Id { get; set; }

        public string Caja { get; set; } = string.Empty;
        public double NumeroArqueo { get; set; }

        public double Valor { get; set; } // Siempre negativo
        public string Concepto { get; set; } = string.Empty; // Lista desplegable
        public string Observacion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string Usuario { get; set; } = "Sistema";
    }

    [Table("VENDEDORES")]
    public class Vendedor
    {
        [Key]
        [Column("CODVENDEDOR")]
        public int CodVendedor { get; set; }

        [Column("NOMVENDEDOR")]
        public string NomVendedor { get; set; } = string.Empty;
    }

    [Table("TIPOSPAGO")]
    public class TipoPago
    {
        [Key]
        [Column("CODTIPOPAGO")]
        public string CodTipoPago { get; set; } = string.Empty;

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; } = string.Empty;
    }
}