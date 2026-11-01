
using EstiloCorte.Application.DTOs;
using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Interfaces
{
    public interface ICitaService
    {
        Task<Cita> CrearAsync(CrearCitaDto dto);
        Task<Cita?> ObtenerPorIdAsync(int id);
        Task<List<Cita>> ObtenerEnRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<bool> CancelarAsync(int id);
        Task<bool> ReprogramarAsync(int id, ReprogramarCitaDto dto);
    }
}
