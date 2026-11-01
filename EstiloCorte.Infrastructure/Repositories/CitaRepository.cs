
using EstiloCorte.Application.Interfaces;
using EstiloCorte.Domain.Entities;
using EstiloCorte.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EstiloCorte.Infrastructure.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private readonly ApplicationDbContext _context;

        public CitaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cita?> ObtenerPorIdAsync(int id)
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Barbero)
                .Include(c => c.Servicio)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cita>> ObtenerPorBarberoEnRangoAsync(
            int barberoId,
            DateTime fechaInicio,
            DateTime fechaFin,
            int? excluirCitaId = null)
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Where(c =>
                    c.BarberoId == barberoId &&
                    c.FechaHoraInicio < fechaFin &&
                    c.FechaHoraFin > fechaInicio &&
                     c.Estado != "Cancelada" &&
            (excluirCitaId == null || c.Id != excluirCitaId))
        .ToListAsync();
        }

        public async Task AgregarAsync(Cita cita)
        {
            await _context.Citas.AddAsync(cita);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
