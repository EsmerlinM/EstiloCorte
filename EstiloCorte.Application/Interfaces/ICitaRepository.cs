
using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Interfaces
{
    public interface ICitaRepository
    {
        Task<Cita?> ObtenerPorIdAsync(int id);

        Task<List<Cita>> ObtenerPorBarberoEnRangoAsync(
            int barberoId,
            DateTime fechaInicio,
            DateTime fechaFin,
            int? excluirCitaId = null);

        Task AgregarAsync(Cita cita);

        Task GuardarCambiosAsync();
    }
}
