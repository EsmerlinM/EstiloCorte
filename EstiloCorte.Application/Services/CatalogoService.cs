using EstiloCorte.Application.DTOs;
using EstiloCorte.Application.Interfaces;
using EstiloCorte.Domain.Entities;

namespace EstiloCorte.Application.Services;

public class CatalogoService : ICatalogoService
{
    private readonly ICatalogoRepository _catalogoRepository;

    public CatalogoService(ICatalogoRepository catalogoRepository)
    {
        _catalogoRepository = catalogoRepository;
    }

    public Task<List<Cliente>> ObtenerClientesActivosAsync()
    {
        return _catalogoRepository.ObtenerClientesActivosAsync();
    }

    public Task<List<Barbero>> ObtenerBarberosActivosAsync()
    {
        return _catalogoRepository.ObtenerBarberosActivosAsync();
    }

    public Task<List<Servicio>> ObtenerServiciosActivosAsync()
    {
        return _catalogoRepository.ObtenerServiciosActivosAsync();
    }

    public async Task<Cliente> CrearClienteAsync(CrearClienteDto dto)
    {
        var cliente = new Cliente
        {
            Nombre = dto.Nombre.Trim(),
            Telefono = dto.Telefono.Trim(),
            Correo = dto.Correo?.Trim()
        };

        await _catalogoRepository.AgregarAsync(cliente);
        await _catalogoRepository.GuardarCambiosAsync();
        return cliente;
    }

    public async Task<Barbero> CrearBarberoAsync(CrearBarberoDto dto)
    {
        var barbero = new Barbero
        {
            Nombre = dto.Nombre.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Correo = dto.Correo?.Trim()
        };

        await _catalogoRepository.AgregarAsync(barbero);
        await _catalogoRepository.GuardarCambiosAsync();
        return barbero;
    }

    public async Task<Servicio> CrearServicioAsync(CrearServicioDto dto)
    {
        var servicio = new Servicio
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            Precio = dto.Precio,
            DuracionMinutos = dto.DuracionMinutos
        };

        await _catalogoRepository.AgregarAsync(servicio);
        await _catalogoRepository.GuardarCambiosAsync();
        return servicio;
    }
}
