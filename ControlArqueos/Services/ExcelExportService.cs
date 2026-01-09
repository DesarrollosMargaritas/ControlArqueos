using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using TesoreriaMargaritas.Models;

namespace TesoreriaMargaritas.Services
{
    public class ExcelExportService
    {
        public byte[] GenerarReporteArqueos(List<ArqueoGridDto> datos, List<string> encabezadosPagos, string tituloReporte)
        {
            // Configurar contexto de licencia (Requerido por EPPlus 5+)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Arqueos");

                // --- ESTILOS ---
                var headerStyle = worksheet.Cells["A1:Z1"].Style; // Rango estimado
                headerStyle.Font.Bold = true;
                headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9F1C0A")); // Rojo Corporativo
                headerStyle.Font.Color.SetColor(Color.White);

                // --- ENCABEZADOS ---
                int col = 1;

                // Columnas Fijas Iniciales
                worksheet.Cells[1, col++].Value = "Fecha";
                worksheet.Cells[1, col++].Value = "Hora";
                worksheet.Cells[1, col++].Value = "Caja";
                worksheet.Cells[1, col++].Value = "Cajero";
                worksheet.Cells[1, col++].Value = "EFECTIVO (Venta)";

                // Columnas Dinámicas (Formas de Pago)
                foreach (var pago in encabezadosPagos)
                {
                    worksheet.Cells[1, col++].Value = pago;
                }

                // Columnas Fijas Finales
                worksheet.Cells[1, col++].Value = "Total Ventas";
                worksheet.Cells[1, col++].Value = "Base";
                worksheet.Cells[1, col++].Value = "Gastos";
                worksheet.Cells[1, col++].Value = "Propinas";
                worksheet.Cells[1, col++].Value = "Anticipos";
                worksheet.Cells[1, col++].Value = "Calculado";
                worksheet.Cells[1, col++].Value = "Declarado";
                worksheet.Cells[1, col++].Value = "Descuadre (Sis)";
                worksheet.Cells[1, col++].Value = "Desc. Neto";
                worksheet.Cells[1, col++].Value = "Compensado";
                worksheet.Cells[1, col++].Value = "Auditado";
                worksheet.Cells[1, col++].Value = "Asegurado";
                worksheet.Cells[1, col++].Value = "Ef. Entregado";

                // --- DATOS ---
                int row = 2;
                foreach (var item in datos)
                {
                    col = 1;
                    worksheet.Cells[row, col++].Value = item.Fecha.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, col++].Value = item.Hora.ToString(@"hh\:mm");
                    worksheet.Cells[row, col++].Value = item.NumeroArqueo; // O nombre de caja si lo tuviéramos en DTO
                    worksheet.Cells[row, col++].Value = item.NombreCajero;
                    worksheet.Cells[row, col++].Value = item.Ef_Ventas;

                    // Dinámicos
                    foreach (var pagoHeader in encabezadosPagos)
                    {
                        var valor = item.DesglosePagos.ContainsKey(pagoHeader) ? item.DesglosePagos[pagoHeader] : 0;
                        worksheet.Cells[row, col++].Value = valor;
                    }

                    // Fijos Finales
                    worksheet.Cells[row, col++].Value = item.TotalVentasNetas;
                    worksheet.Cells[row, col++].Value = item.Ef_Base;
                    worksheet.Cells[row, col++].Value = item.Ef_Gastos;
                    worksheet.Cells[row, col++].Value = item.Ef_Propinas;
                    worksheet.Cells[row, col++].Value = item.Ef_Anticipos;
                    worksheet.Cells[row, col++].Value = item.Ef_Calculado;
                    worksheet.Cells[row, col++].Value = item.Ef_Declarado;

                    // Colorear Descuadre
                    var cellDesc = worksheet.Cells[row, col];
                    cellDesc.Value = item.Ef_Descuadre;
                    if (item.Ef_Descuadre < 0) cellDesc.Style.Font.Color.SetColor(Color.Red);
                    col++;

                    worksheet.Cells[row, col++].Value = item.Ef_DescuadreFinal;
                    worksheet.Cells[row, col++].Value = item.TotalCompensado;

                    var cellAudit = worksheet.Cells[row, col];
                    cellAudit.Value = item.Ef_DescuadreAuditado;
                    if (item.Ef_DescuadreAuditado == 0 && item.TotalCompensado != 0) cellAudit.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                    col++;

                    worksheet.Cells[row, col++].Value = item.Ef_Asegurado;
                    worksheet.Cells[row, col++].Value = item.Ef_EfectivoEntregado;

                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}