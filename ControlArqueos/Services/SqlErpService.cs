using Microsoft.EntityFrameworkCore;
using TesoreriaMargaritas.Data;
using TesoreriaMargaritas.Models;

namespace TesoreriaMargaritas.Services
{
    public class SqlErpService : IErpService
    {
        private readonly ApplicationDbContext _context;

        public SqlErpService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RemFront>> ObtenerPuntosActivosConCajasAsync()
        {
            // Consulta Real a SQL Server:
            // 1. Traemos los Fronts donde Descatalogado sea false (0)
            // 2. Incluimos (JOIN) las Cajas asociadas
            return await _context.PuntosDeVenta
                .Where(p => !p.Descatalogado)
                .Include(p => p.Cajas)
                .AsNoTracking() // Recomendado para solo lectura, mejora rendimiento
                .ToListAsync();
        }

        // Método auxiliar para obtener el detalle de una caja específica (para la vista detalle)
        public async Task<RemCajaFront?> ObtenerCajaPorIdAsync(int idCaja)
        {
            return await _context.Cajas
                .FirstOrDefaultAsync(c => c.CajaFront == idCaja);
        }
    }
}