namespace EstiloCorte.API.DTOs;

public class CitaRespuestaDto
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string? ClienteNombre { get; set; }

    public int BarberoId { get; set; }

    public string? BarberoNombre { get; set; }

    public int ServicioId { get; set; }

    public string? ServicioNombre { get; set; }

    public DateTime FechaHoraInicio { get; set; }

    public DateTime FechaHoraFin { get; set; }

    public string Estado { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }
}
