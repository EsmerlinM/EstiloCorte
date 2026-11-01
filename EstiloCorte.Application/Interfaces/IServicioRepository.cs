
using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Interfaces
{
    public interface IServicioRepository
    {
        Task<Servicio?> ObtenerPorIdAsync(int id);
    }
}
