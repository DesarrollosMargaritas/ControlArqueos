using TesoreriaMargaritas.Models;

namespace TesoreriaMargaritas.Services
{
    public interface IErpService
    {
        Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync();

        // Método para la vista del día a día (limitado)
        Task<ReporteArqueosResponse> ObtenerArqueosUltimos30Dias(int idFront, int idCajaFront);

        // NUEVO: Método sin límites para reportes históricos
        Task<ReporteArqueosResponse> ObtenerReporteArqueosPorRango(int idFront, int idCajaFront, DateTime fechaInicio, DateTime fechaFin);

        Task<RemCajaFront?> ObtenerCajaPorIdAsync(int idFront, int idCajaFront);
        Task<bool> AgregarCompensacionAsync(CompensacionArqueo compensacion);
        Task<bool> AgregarPagoCierreAsync(PagoCierre pago);
    }

    public class SimulacionErpService : IErpService
    {
        // ... (Implementaciones dummy existentes) ...
        public async Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync() => new List<RemFront>();
        public async Task<RemCajaFront?> ObtenerCajaPorIdAsync(int idFront, int idCaja) => null;
        public async Task<ReporteArqueosResponse> ObtenerArqueosUltimos30Dias(int idFront, int idCajaFront) => new ReporteArqueosResponse();
        public async Task<bool> AgregarCompensacionAsync(CompensacionArqueo compensacion) => true;
        public async Task<bool> AgregarPagoCierreAsync(PagoCierre pago) => true;

        // Implementación Dummy del nuevo método
        public async Task<ReporteArqueosResponse> ObtenerReporteArqueosPorRango(int idFront, int idCajaFront, DateTime fechaInicio, DateTime fechaFin)
        {
            await Task.Delay(100);
            return new ReporteArqueosResponse();
        }
    }
}