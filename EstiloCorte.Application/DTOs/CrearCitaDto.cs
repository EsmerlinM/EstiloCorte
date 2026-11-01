
namespace EstiloCorte.Application.DTOs
{
    public class CrearCitaDto
    {
        public int ClienteId { get; set; }

        public int BarberoId { get; set; }

        public int ServicioId { get; set; }

        public DateTime FechaHoraInicio { get; set; }
    }
}
