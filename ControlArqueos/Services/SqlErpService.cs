using Microsoft.EntityFrameworkCore;
using TesoreriaMargaritas.Data;
using TesoreriaMargaritas.Models;
using System.Globalization;

namespace TesoreriaMargaritas.Services
{
    public class SqlErpService : IErpService
    {
        private readonly ApplicationDbContext _contextErp; // ERP
        private readonly AuditDbContext _contextAudit;     // Auditoría

        public SqlErpService(ApplicationDbContext contextErp, AuditDbContext contextAudit)
        {
            _contextErp = contextErp;
            _contextAudit = contextAudit;
        }

        public async Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync()
        {
            return await _contextErp.PuntosDeVenta
                .Where(p => !p.Descatalogado)
                .Include(p => p.Cajas)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<RemCajaFront?> ObtenerCajaPorIdAsync(int idFront, int idCaja)
        {
            return await _contextErp.Cajas
                .FirstOrDefaultAsync(c => c.IdFront == idFront && c.CajaFront == idCaja);
        }

        public async Task<bool> AgregarCompensacionAsync(CompensacionArqueo compensacion)
        {
            try
            {
                _contextAudit.Compensaciones.Add(compensacion);
                await _contextAudit.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando compensación: {ex.Message}");
                return false;
            }
        }

        // NUEVO: Guardar Pago Posterior
        public async Task<bool> AgregarPagoCierreAsync(PagoCierre pago)
        {
            try
            {
                _contextAudit.PagosCierre.Add(pago);
                await _contextAudit.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando pago cierre: {ex.Message}");
                return false;
            }
        }

        public async Task<ReporteArqueosResponse> ObtenerArqueosUltimos30Dias(int idFront, int idCajaFront)
        {
            var respuesta = new ReporteArqueosResponse();

            var cajaConfig = await _contextErp.Cajas
                .Where(c => c.IdFront == idFront && c.CajaFront == idCajaFront)
                .Select(c => c.CajaManager)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(cajaConfig))
                return respuesta;

            var fechaLimite = DateTime.Today.AddDays(-30);

            // --- QUERY AL ERP ---
            var sqlQuery = $@"
                SELECT 
                    a.Fecha,
                    a.Hora,
                    CAST(a.Numero AS float) as Numero,
                    v.NOMVENDEDOR as NombreCajero,
                    l.CODTIPOPAGO as CodTipoPago,
                    t.DESCRIPCION as DescTipoPago,
                    
                    CAST(l.Ventas AS float) as Ventas,
                    CAST(l.Abonos AS float) as Abonos,
                    
                    CAST(l.Fianza AS float) as Fianza,
                    CAST(l.Pagos AS float) as Pagos,
                    CAST(l.Anticipos AS float) as Anticipos,
                    CAST(l.Calculado AS float) as Calculado,
                    CAST(l.Declarado AS float) as Declarado,
                    CAST(l.Descuadre AS float) as Descuadre,
                    CAST(l.Retirado AS float) as Retirado,
                    
                    (
                        SELECT ISNULL(SUM(CAST(f.PROPINA AS float)), 0)
                        FROM FACTURASVENTA f
                        WHERE f.CAJA = a.Caja AND f.Z = CAST(a.Numero AS int)
                    ) as TotalPropinas

                FROM ARQUEOS a
                LEFT JOIN VENDEDORES v ON a.CODVENDEDOR = v.CODVENDEDOR
                JOIN ARQUEOSLIN l ON a.Caja = l.Caja AND a.Numero = l.Numero
                LEFT JOIN TIPOSPAGO t ON l.CODTIPOPAGO = t.CODTIPOPAGO
                WHERE a.Caja = {{0}} 
                  AND a.Fecha >= {{1}}
                ORDER BY a.Fecha DESC, a.Hora DESC";

            var flatData = await _contextErp.Database.SqlQueryRaw<ArqueoFlatResult>(
                sqlQuery, cajaConfig, fechaLimite
            ).ToListAsync();

            // --- QUERY A AUDITORÍA (BD SEPARADA) ---
            var numerosArqueo = flatData.Select(x => x.Numero).Distinct().ToList();

            // 1. Traer Compensaciones
            var compensacionesDb = await _contextAudit.Compensaciones
                .Where(c => c.Caja == cajaConfig && numerosArqueo.Contains(c.NumeroArqueo))
                .ToListAsync();

            // 2. Traer Pagos Posteriores (NUEVO)
            var pagosCierreDb = await _contextAudit.PagosCierre
                .Where(p => p.Caja == cajaConfig && numerosArqueo.Contains(p.NumeroArqueo))
                .ToListAsync();

            var tiposPagoEncontrados = new HashSet<string>();
            var gruposArqueo = flatData.GroupBy(x => new { x.Fecha, x.Numero });

            foreach (var grupo in gruposArqueo)
            {
                var primerReg = grupo.First();

                TimeSpan horaParsed = TimeSpan.Zero;
                if (!string.IsNullOrEmpty(primerReg.Hora))
                {
                    if (DateTime.TryParse(primerReg.Hora, out DateTime dtHora)) horaParsed = dtHora.TimeOfDay;
                    else if (TimeSpan.TryParse(primerReg.Hora, out TimeSpan tsHora)) horaParsed = tsHora;
                }

                var fila = new ArqueoGridDto
                {
                    Fecha = primerReg.Fecha,
                    Hora = horaParsed,
                    NumeroArqueo = primerReg.Numero,
                    NombreCajero = primerReg.NombreCajero ?? "Desconocido",
                    TotalVentasNetas = 0,
                    Ef_Propinas = primerReg.TotalPropinas
                };

                // Asignar Compensaciones
                var comps = compensacionesDb.Where(c => c.NumeroArqueo == primerReg.Numero).ToList();
                fila.Compensaciones = comps.Select(c => new CompensacionDto
                {
                    Id = c.Id,
                    Valor = c.Valor,
                    Observacion = c.Observacion,
                    FechaRegistro = c.FechaRegistro
                }).ToList();
                fila.TotalCompensado = comps.Sum(c => c.Valor);

                // Asignar Pagos Posteriores (NUEVO)
                var pagos = pagosCierreDb.Where(p => p.NumeroArqueo == primerReg.Numero).ToList();
                fila.PagosPosteriores = pagos.Select(p => new PagoCierreDto
                {
                    Id = p.Id,
                    Valor = p.Valor,
                    Concepto = p.Concepto,
                    Observacion = p.Observacion,
                    FechaRegistro = p.FechaRegistro
                }).ToList();
                fila.TotalPagosPosteriores = pagos.Sum(p => p.Valor);

                foreach (var linea in grupo)
                {
                    var valorNeto = linea.Ventas - linea.Abonos;

                    if (linea.CodTipoPago == "1") // EFECTIVO
                    {
                        fila.Ef_Base += linea.Fianza;
                        fila.Ef_Gastos += linea.Pagos;
                        fila.Ef_Anticipos += linea.Anticipos;
                        fila.Ef_Calculado += linea.Calculado;
                        fila.Ef_Declarado += linea.Declarado;
                        fila.Ef_Descuadre += linea.Descuadre;
                        fila.Ef_Asegurado += linea.Retirado;

                        fila.Ef_Ventas += valorNeto;
                        fila.TotalVentasNetas += valorNeto;
                    }
                    else
                    {
                        var nombrePago = linea.DescTipoPago ?? "OTROS";

                        if (linea.CodTipoPago == "-1")
                            nombrePago = "LEGALIZACIONES";
                        else if (nombrePago == "OTROS" && !string.IsNullOrEmpty(linea.CodTipoPago))
                            nombrePago = $"COD-{linea.CodTipoPago}";

                        if (fila.DesglosePagos.ContainsKey(nombrePago))
                            fila.DesglosePagos[nombrePago] += valorNeto;
                        else
                            fila.DesglosePagos.Add(nombrePago, valorNeto);

                        fila.TotalVentasNetas += valorNeto;
                        tiposPagoEncontrados.Add(nombrePago);
                    }
                }

                fila.Ef_DescuadreFinal = fila.Ef_Descuadre - fila.Ef_Propinas;
                fila.Ef_DescuadreAuditado = fila.Ef_DescuadreFinal + fila.TotalCompensado;

                // Cálculo Final Efectivo Entregado = Asegurado + TotalPagos (se suma porque TotalPagos es negativo)
                fila.Ef_EfectivoEntregado = fila.Ef_Asegurado + fila.TotalPagosPosteriores;

                respuesta.Filas.Add(fila);
            }

            respuesta.EncabezadosPagos = tiposPagoEncontrados.OrderBy(x => x).ToList();
            return respuesta;
        }
    }

    public class ArqueoFlatResult
    {
        public DateTime Fecha { get; set; }
        public string? Hora { get; set; }
        public double Numero { get; set; }
        public string? NombreCajero { get; set; }
        public string? CodTipoPago { get; set; }
        public string? DescTipoPago { get; set; }
        public double Ventas { get; set; }
        public double Abonos { get; set; }
        public double Fianza { get; set; }
        public double Pagos { get; set; }
        public double Anticipos { get; set; }
        public double Calculado { get; set; }
        public double Declarado { get; set; }
        public double Descuadre { get; set; }
        public double Retirado { get; set; }
        public double TotalPropinas { get; set; }
    }
}