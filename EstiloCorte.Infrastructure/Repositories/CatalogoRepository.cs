using EstiloCorte.Application.Interfaces;
using EstiloCorte.Domain.Entities;
using EstiloCorte.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EstiloCorte.Infrastructure.Repositories;

public class CatalogoRepository : ICatalogoRepository
{
    private readonly ApplicationDbContext _context;

    public CatalogoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Cliente>> ObtenerClientesActivosAsync()
    {
        return _context.Clientes
            .AsNoTracking()
            .Where(cliente => cliente.Activo)
            .OrderBy(cliente => cliente.Nombre)
            .ToListAsync();
    }

    public Task<List<Barbero>> ObtenerBarberosActivosAsync()
    {
        return _context.Barberos
            .AsNoTracking()
            .Where(barbero => barbero.Activo)
            .OrderBy(barbero => barbero.Nombre)
            .ToListAsync();
    }

    public Task<List<Servicio>> ObtenerServiciosActivosAsync()
    {
        return _context.Servicios
            .AsNoTracking()
            .Where(servicio => servicio.Activo)
            .OrderBy(servicio => servicio.Nombre)
            .ToListAsync();
    }

    public async Task AgregarAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
    }

    public async Task AgregarAsync(Barbero barbero)
    {
        await _context.Barberos.AddAsync(barbero);
    }

    public async Task AgregarAsync(Servicio servicio)
    {
        await _context.Servicios.AddAsync(servicio);
    }

    public Task GuardarCambiosAsync()
    {
        return _context.SaveChangesAsync();
    }
}
