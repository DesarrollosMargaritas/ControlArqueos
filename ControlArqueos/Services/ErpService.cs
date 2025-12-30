using TesoreriaMargaritas.Models;

namespace TesoreriaMargaritas.Services
{
    // Interfaz para definir qué hace el servicio (útil para luego cambiar a SQL real sin romper la vista)
    public interface IErpService
    {
        Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync();
    }

    public class SimulacionErpService : IErpService
    {
        public async Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync()
        {
            // Simulamos una pequeña espera de base de datos
            await Task.Delay(100);

            // 1. Simulamos datos de REM_FRONTS
            var todosLosFronts = new List<RemFront>
            {
                new RemFront { IdFront = 1, Titulo = "Sede Principal", Descatalogado = false },
                new RemFront { IdFront = 2, Titulo = "Sucursal Norte", Descatalogado = false },
                new RemFront { IdFront = 99, Titulo = "Sede Antigua (Cerrada)", Descatalogado = true } // Este NO debería salir
            };

            // 2. Simulamos datos de REM_CAJASFRONT
            var todasLasCajas = new List<RemCajaFront>
            {
                // Cajas de Sede Principal
                new RemCajaFront { CajaFront = 101, IdFront = 1, Descripcion = "Caja General 1" },
                new RemCajaFront { CajaFront = 102, IdFront = 1, Descripcion = "Caja Rápida" },
                
                // Cajas de Sucursal Norte
                new RemCajaFront { CajaFront = 201, IdFront = 2, Descripcion = "Caja Barra" },
                
                // Cajas de Sede Cerrada
                new RemCajaFront { CajaFront = 999, IdFront = 99, Descripcion = "Caja Vieja" }
            };

            // 3. Lógica de "Join" y Filtrado (Lo que luego hará SQL)
            // Filtramos solo los NO descatalogados
            var puntosActivos = todosLosFronts.Where(f => !f.Descatalogado).ToList();

            // Asignamos las cajas correspondientes a cada punto
            foreach (var punto in puntosActivos)
            {
                punto.Cajas = todasLasCajas.Where(c => c.IdFront == punto.IdFront).ToList();
            }

            return puntosActivos;
        }
    }
}