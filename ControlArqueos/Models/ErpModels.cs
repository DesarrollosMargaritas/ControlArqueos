using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesoreriaMargaritas.Models
{
    // Representa la tabla REM_FRONTS
    public class RemFront
    {
        [Key]
        public int IdFront { get; set; } // PK

        public string Titulo { get; set; } = string.Empty; // Nombre a desplegar

        public bool Descatalogado { get; set; } // 0 = Activo, 1 = Inactivo

        // Relación: Un punto de venta tiene muchas cajas
        public List<RemCajaFront> Cajas { get; set; } = new List<RemCajaFront>();
    }

    // Representa la tabla REM_CAJASFRONT
    public class RemCajaFront
    {
        [Key]
        public int CajaFront { get; set; } // PK

        public int IdFront { get; set; } // FK hacia REM_FRONTS

        public string Descripcion { get; set; } = string.Empty; // Nombre de la caja
    }
}