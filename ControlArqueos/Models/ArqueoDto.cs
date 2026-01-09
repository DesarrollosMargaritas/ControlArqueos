using System;
using System.Collections.Generic;

namespace TesoreriaMargaritas.Models
{
    // --- OBJETOS DE TRANSFERENCIA DE DATOS (DTOs) ---

    public class ArqueoGridDto
    {
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public double NumeroArqueo { get; set; }
        public string NombreCajero { get; set; } = string.Empty;
        public double TotalVentasNetas { get; set; }

        // Sección 1
        public double Ef_Ventas { get; set; }
        public Dictionary<string, double> DesglosePagos { get; set; } = new Dictionary<string, double>();

        // Sección 2
        public double Ef_Base { get; set; }
        public double Ef_Gastos { get; set; }
        public double Ef_Propinas { get; set; }
        public double Ef_Anticipos { get; set; }
        public double Ef_Calculado { get; set; }
        public double Ef_Declarado { get; set; }
        public double Ef_Descuadre { get; set; }

        // Calculados
        public double Ef_DescuadreFinal { get; set; }

        // Auditoría (Compensaciones Descuadre)
        public List<CompensacionDto> Compensaciones { get; set; } = new List<CompensacionDto>();
        public double TotalCompensado { get; set; }
        public double Ef_DescuadreAuditado { get; set; }

        public double Ef_Asegurado { get; set; }

        // NUEVO: Control de Gastos (Pagos Posteriores)
        public List<PagoCierreDto> PagosPosteriores { get; set; } = new List<PagoCierreDto>();
        public double TotalPagosPosteriores { get; set; } // Suma de los gastos (valor negativo)
        public double Ef_EfectivoEntregado { get; set; } // Asegurado + TotalPagosPosteriores (resta porque es negativo)
    }

    public class ReporteArqueosResponse
    {
        public List<ArqueoGridDto> Filas { get; set; } = new List<ArqueoGridDto>();
        public List<string> EncabezadosPagos { get; set; } = new List<string>();
    }

    public class CompensacionDto
    {
        public int Id { get; set; }
        public double Valor { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }

    // NUEVO DTO
    public class PagoCierreDto
    {
        public int Id { get; set; }
        public double Valor { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}