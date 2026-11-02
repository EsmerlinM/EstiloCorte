using System.ComponentModel.DataAnnotations;

namespace EstiloCorte.Application.DTOs;

public class CrearServicioDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal Precio { get; set; }

    [Range(1, 600)]
    public int DuracionMinutos { get; set; }
}
