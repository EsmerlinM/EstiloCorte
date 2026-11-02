
using EstiloCorte.Application.DTOs;
using EstiloCorte.Application.Exceptions;
using EstiloCorte.Application.Interfaces;
using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Services
{
    public class CitaService : ICitaService
    {
        private static readonly TimeSpan HoraApertura = new(9, 0, 0);
        private static readonly TimeSpan HoraCierre = new(18, 0, 0);
        private static readonly TimeSpan IntervaloSugerencias = TimeSpan.FromMinutes(15);
        private const int CantidadHorariosSugeridos = 3;
        private const int DiasBusquedaHorarios = 14;

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
                var horariosSugeridos = await ObtenerHorariosSugeridosAsync(
                    dto.BarberoId,
                    dto.FechaHoraInicio,
                    servicio.DuracionMinutos,
                    citasExistentes);

                throw new ConflictoHorarioException(
                    "El barbero ya tiene una cita en ese horario.",
                    horariosSugeridos);
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

        public Task<List<Cita>> ObtenerEnRangoAsync(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            return _citaRepository.ObtenerEnRangoAsync(fechaInicio, fechaFin);
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
            {
                var horariosSugeridos = await ObtenerHorariosSugeridosAsync(
                    dto.BarberoId,
                    dto.FechaHoraInicio,
                    servicio.DuracionMinutos,
                    conflictos,
                    id);

                throw new ConflictoHorarioException(
                    "El barbero ya tiene una cita en ese horario.",
                    horariosSugeridos);
            }

            cita.BarberoId = dto.BarberoId;
            cita.ServicioId = dto.ServicioId;
            cita.FechaHoraInicio = dto.FechaHoraInicio;
            cita.FechaHoraFin = fechaFin;

            await _citaRepository.GuardarCambiosAsync();

            return true;
        }

        private async Task<IReadOnlyList<DateTime>> ObtenerHorariosSugeridosAsync(
            int barberoId,
            DateTime fechaSolicitada,
            int duracionMinutos,
            IReadOnlyCollection<Cita> conflictos,
            int? excluirCitaId = null)
        {
            var fechaFinBusqueda = fechaSolicitada.Date.AddDays(DiasBusquedaHorarios);
            var citas = await _citaRepository.ObtenerPorBarberoEnRangoAsync(
                barberoId,
                fechaSolicitada.Date,
                fechaFinBusqueda,
                excluirCitaId);

            var primerHorario = conflictos.Max(cita => cita.FechaHoraFin)
                .Add(IntervaloSugerencias);
            var sugerencias = new List<DateTime>(CantidadHorariosSugeridos);
            var limiteIntentos = DiasBusquedaHorarios * 24 * 4;

            for (var intento = 0;
                 intento < limiteIntentos && sugerencias.Count < CantidadHorariosSugeridos;
                 intento++)
            {
                primerHorario = AjustarHorarioLaboral(primerHorario, duracionMinutos);
                if (primerHorario >= fechaFinBusqueda)
                {
                    break;
                }

                var fechaFin = primerHorario.AddMinutes(duracionMinutos);
                var conflicto = citas
                    .Where(cita =>
                        cita.FechaHoraInicio < fechaFin &&
                        cita.FechaHoraFin > primerHorario)
                    .OrderByDescending(cita => cita.FechaHoraFin)
                    .FirstOrDefault();

                if (conflicto is not null)
                {
                    primerHorario = conflicto.FechaHoraFin.Add(IntervaloSugerencias);
                    continue;
                }

                sugerencias.Add(primerHorario);
                primerHorario = primerHorario.Add(IntervaloSugerencias);
            }

            return sugerencias;
        }

        private static DateTime AjustarHorarioLaboral(DateTime horario, int duracionMinutos)
        {
            while (true)
            {
                if (horario.DayOfWeek == DayOfWeek.Sunday ||
                    horario.TimeOfDay < HoraApertura)
                {
                    horario = horario.Date.AddDays(
                        horario.DayOfWeek == DayOfWeek.Sunday ? 1 : 0) + HoraApertura;
                    continue;
                }

                if (horario.AddMinutes(duracionMinutos).TimeOfDay > HoraCierre)
                {
                    horario = horario.Date.AddDays(1) + HoraApertura;
                    continue;
                }

                return horario;
            }
        }
    }
}
