using TesoreriaMargaritas.Models;

namespace TesoreriaMargaritas.Services
{
    public interface IErpService
    {
        Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync();
        Task<ReporteArqueosResponse> ObtenerArqueosUltimos30Dias(int idFront, int idCajaFront);
        Task<RemCajaFront?> ObtenerCajaPorIdAsync(int idFront, int idCajaFront);
        Task<bool> AgregarCompensacionAsync(CompensacionArqueo compensacion);

        // NUEVO
        Task<bool> AgregarPagoCierreAsync(PagoCierre pago);
    }

    public class SimulacionErpService : IErpService
    {
        public async Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync()
        {
            await Task.Delay(100);
            return new List<RemFront>();
        }

        public async Task<RemCajaFront?> ObtenerCajaPorIdAsync(int idFront, int idCaja)
        {
            await Task.Delay(50);
            return new RemCajaFront
            {
                IdFront = idFront,
                CajaFront = idCaja,
                Descripcion = "Caja Simulada",
                CajaManager = "CJ0"
            };
        }

        public async Task<ReporteArqueosResponse> ObtenerArqueosUltimos30Dias(int idFront, int idCajaFront)
        {
            await Task.Delay(50);
            return new ReporteArqueosResponse();
        }

        public async Task<bool> AgregarCompensacionAsync(CompensacionArqueo compensacion)
        {
            await Task.Delay(100);
            return true;
        }

        // NUEVO DUMMY
        public async Task<bool> AgregarPagoCierreAsync(PagoCierre pago)
        {
            await Task.Delay(100);
            return true;
        }
    }
}