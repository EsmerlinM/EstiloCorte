

namespace EstiloCorte.Domain.Entities
{
    public class Cita
    {
        public int Id { get; set; }
       
        public int ClienteId { get; set; }
       
        public int BarberoId { get; set; }

        public int ServicioId { get; set; }
       
        public DateTime FechaHoraInicio {  get; set; }

        public DateTime FechaHoraFin { get; set; }

        public string Estado { get; set; } = "pendiente";

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Cliente Cliente { get; set; } = null!;

        public Barbero Barbero { get; set; } = null!;

        public Servicio Servicio { get; set; } = null!;
    }
}
