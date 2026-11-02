
using EstiloCorte.Application.Interfaces;
using EstiloCorte.Domain.Entities;
using EstiloCorte.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EstiloCorte.Infrastructure.Repositories
{
    public class ServicioRepository : IServicioRepository
    {
        private readonly ApplicationDbContext _context;

        public ServicioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Servicio?> ObtenerPorIdAsync(int id)
        {
            return await _context.Servicios
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
