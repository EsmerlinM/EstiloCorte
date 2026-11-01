
using EstiloCorte.Application.DTOs;
using EstiloCorte.Application.Interfaces;
using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IServicioRepository _servicioRepository;

        public CitaService(
            ICitaRepository citaRepository,
            IServicioRepository servicioRepository)
        {
            _citaRepository = citaRepository;
            _servicioRepository = servicioRepository;
        }

        public async Task<Cita> CrearAsync(CrearCitaDto dto)
        {
            // 1. Buscar el servicio solicitado
            var servicio = await _servicioRepository
                .ObtenerPorIdAsync(dto.ServicioId);

            if (servicio == null || !servicio.Activo)
            {
                throw new InvalidOperationException(
                    "El servicio no existe o no está disponible.");
            }

            // 2. Calcular la hora de finalización
            var fechaHoraFin = dto.FechaHoraInicio
                .AddMinutes(servicio.DuracionMinutos);

            // 3. Consultar las citas que se cruzan con ese horario
            var citasExistentes = await _citaRepository
                .ObtenerPorBarberoEnRangoAsync(
                    dto.BarberoId,
                    dto.FechaHoraInicio,
                    fechaHoraFin);

            // 4. Comprobar si existe un conflicto
            if (citasExistentes.Any())
            {
                throw new InvalidOperationException(
                    "El barbero ya tiene una cita en ese horario.");
            }

            // 5. Crear la nueva cita
            var cita = new Cita
            {
                ClienteId = dto.ClienteId,
                BarberoId = dto.BarberoId,
                ServicioId = dto.ServicioId,
                FechaHoraInicio = dto.FechaHoraInicio,
                FechaHoraFin = fechaHoraFin,
                Estado = "Pendiente",
                FechaCreacion = DateTime.Now
            };

            // 6. Guardar la cita
            await _citaRepository.AgregarAsync(cita);
            await _citaRepository.GuardarCambiosAsync();

            return cita;
        }
        public async Task<Cita?> ObtenerPorIdAsync(int id)
        {
            return await _citaRepository.ObtenerPorIdAsync(id);
        }

        public async Task<bool> CancelarAsync(int id)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(id);

            if (cita == null)
                return false;

            if (cita.Estado == "Cancelada")
                throw new InvalidOperationException("La cita ya está cancelada.");

            cita.Estado = "Cancelada";

            await _citaRepository.GuardarCambiosAsync();

            return true;
        }

        public async Task<bool> ReprogramarAsync(
             int id,
             ReprogramarCitaDto dto)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(id);

            if (cita == null)
                return false;

            if (cita.Estado == "Cancelada")
                throw new InvalidOperationException(
                    "No se puede reprogramar una cita cancelada.");

            var servicio = await _servicioRepository
                .ObtenerPorIdAsync(dto.ServicioId);

            if (servicio == null || !servicio.Activo)
                throw new InvalidOperationException(
                    "El servicio no existe o no está disponible.");

            var fechaFin = dto.FechaHoraInicio
                .AddMinutes(servicio.DuracionMinutos);

            var conflictos = await _citaRepository
                .ObtenerPorBarberoEnRangoAsync(
                    dto.BarberoId,
                    dto.FechaHoraInicio,
                    fechaFin,
                    id);

            if (conflictos.Any())
                throw new InvalidOperationException(
                    "El barbero ya tiene una cita en ese horario.");

            cita.BarberoId = dto.BarberoId;
            cita.ServicioId = dto.ServicioId;
            cita.FechaHoraInicio = dto.FechaHoraInicio;
            cita.FechaHoraFin = fechaFin;

            await _citaRepository.GuardarCambiosAsync();

            return true;
        }
    }
}
