using System.ComponentModel.DataAnnotations;

namespace EstiloCorte.Application.DTOs;

public class CrearClienteDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Telefono { get; set; } = string.Empty;

    [EmailAddress, StringLength(150)]
    public string? Correo { get; set; }
}
