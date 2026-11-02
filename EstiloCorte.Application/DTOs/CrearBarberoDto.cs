using System.ComponentModel.DataAnnotations;

namespace EstiloCorte.Application.DTOs;

public class CrearBarberoDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Telefono { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Correo { get; set; }
}
