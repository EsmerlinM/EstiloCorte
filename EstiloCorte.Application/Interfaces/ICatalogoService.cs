using EstiloCorte.Application.DTOs;
using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Interfaces;

public interface ICatalogoService
{
    Task<List<Cliente>> ObtenerClientesActivosAsync();

    Task<List<Barbero>> ObtenerBarberosActivosAsync();

    Task<List<Servicio>> ObtenerServiciosActivosAsync();

    Task<Cliente> CrearClienteAsync(CrearClienteDto dto);

    Task<Barbero> CrearBarberoAsync(CrearBarberoDto dto);

    Task<Servicio> CrearServicioAsync(CrearServicioDto dto);
}
