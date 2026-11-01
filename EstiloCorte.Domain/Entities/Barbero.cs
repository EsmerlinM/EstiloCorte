
namespace EstiloCorte.Domain.Entities
{
    public class Barbero
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Correo { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
