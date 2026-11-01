
using EstiloCorte.Application.DTOs;
using EstiloCorte.Application.Interfaces;
using EstiloCorte.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EstiloCorte.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearCitaDto dto)
        {
            try
            {
                var cita = await _citaService.CrearAsync(dto);

                return StatusCode(201, cita);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var cita = await _citaService.ObtenerPorIdAsync(id);

            if (cita == null)
                return NotFound(new { mensaje = "Cita no encontrada." });

            return Ok(cita);
        }

        [HttpPatch("{id}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var resultado = await _citaService.CancelarAsync(id);

                if (!resultado)
                    return NotFound(new { mensaje = "Cita no encontrada." });

                return Ok(new { mensaje = "Cita cancelada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}/reprogramar")]
        public async Task<IActionResult> Reprogramar(
    int id,
    ReprogramarCitaDto dto)
        {
            try
            {
                var resultado = await _citaService.ReprogramarAsync(id, dto);

                if (!resultado)
                    return NotFound(new { mensaje = "Cita no encontrada." });

                return Ok(new { mensaje = "Cita reprogramada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }
    }
}
