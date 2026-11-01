using EstiloCorte.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EstiloCorte.Application.Interfaces;
using EstiloCorte.Application.Services;
using EstiloCorte.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
// Repositorios
builder.Services.AddScoped<ICitaRepository, CitaRepository>();

builder.Services.AddScoped<IServicioRepository, ServicioRepository>();

// Servicios de aplicación
builder.Services.AddScoped<ICitaService, CitaService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        // Conectamos la interfaz visual con el JSON nativo de .NET 10
        options.SwaggerEndpoint("/openapi/v1.json", "Mi API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
