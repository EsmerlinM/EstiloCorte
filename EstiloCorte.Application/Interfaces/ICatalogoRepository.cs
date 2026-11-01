using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Interfaces;

public interface ICatalogoRepository
{
    Task<List<Cliente>> ObtenerClientesActivosAsync();

    Task<List<Barbero>> ObtenerBarberosActivosAsync();

    Task<List<Servicio>> ObtenerServiciosActivosAsync();

    Task AgregarAsync(Cliente cliente);

    Task AgregarAsync(Barbero barbero);

    Task AgregarAsync(Servicio servicio);

    Task GuardarCambiosAsync();
}
