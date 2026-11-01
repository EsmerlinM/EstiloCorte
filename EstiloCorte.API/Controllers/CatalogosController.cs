using EstiloCorte.Application.DTOs;
using EstiloCorte.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EstiloCorte.API.Controllers;

[ApiController]
[Route("api/catalogos")]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogoService _catalogoService;

    public CatalogosController(ICatalogoService catalogoService)
    {
        _catalogoService = catalogoService;
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> ObtenerClientes()
    {
        var clientes = await _catalogoService.ObtenerClientesActivosAsync();
        return Ok(clientes.Select(cliente => new
        {
            cliente.Id,
            cliente.Nombre,
            cliente.Telefono,
            cliente.Correo
        }));
    }

    [HttpPost("clientes")]
    public async Task<IActionResult> CrearCliente(CrearClienteDto dto)
    {
        var cliente = await _catalogoService.CrearClienteAsync(dto);

        return CreatedAtAction(nameof(ObtenerClientes), new
        {
            cliente.Id,
            cliente.Nombre,
            cliente.Telefono,
            cliente.Correo
        });
    }

    [HttpGet("barberos")]
    public async Task<IActionResult> ObtenerBarberos()
    {
        var barberos = await _catalogoService.ObtenerBarberosActivosAsync();
        return Ok(barberos.Select(barbero => new
        {
            barbero.Id,
            barbero.Nombre,
            barbero.Telefono,
            barbero.Correo
        }));
    }

    [HttpPost("barberos")]
    public async Task<IActionResult> CrearBarbero(CrearBarberoDto dto)
    {
        var barbero = await _catalogoService.CrearBarberoAsync(dto);

        return CreatedAtAction(nameof(ObtenerBarberos), new
        {
            barbero.Id,
            barbero.Nombre,
            barbero.Telefono,
            barbero.Correo
        });
    }

    [HttpGet("servicios")]
    public async Task<IActionResult> ObtenerServicios()
    {
        var servicios = await _catalogoService.ObtenerServiciosActivosAsync();
        return Ok(servicios.Select(servicio => new
        {
            servicio.Id,
            servicio.Nombre,
            servicio.Descripcion,
            servicio.Precio,
            servicio.DuracionMinutos
        }));
    }

    [HttpPost("servicios")]
    public async Task<IActionResult> CrearServicio(CrearServicioDto dto)
    {
        var servicio = await _catalogoService.CrearServicioAsync(dto);

        return CreatedAtAction(nameof(ObtenerServicios), new
        {
            servicio.Id,
            servicio.Nombre,
            servicio.Descripcion,
            servicio.Precio,
            servicio.DuracionMinutos
        });
    }
}
