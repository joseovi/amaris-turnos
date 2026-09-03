using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TurnosBanco.Api.Controllers;

[ApiController]
[Route("api/turnos")]
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _turnoService;

    public TurnosController(ITurnoService turnoService)
    {
        _turnoService = turnoService;
    }

    [HttpPost]
    public async Task<ActionResult<TurnoDto>> Crear(
        [FromBody] CrearTurnoDto dto, CancellationToken ct)
    {
        var turno = await _turnoService.CrearTurnoAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = turno.Id }, turno);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TurnoDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var turno = await _turnoService.ObtenerPorIdAsync(id, ct);
        return Ok(turno);
    }

    [HttpGet]
    public async Task<ActionResult<List<TurnoDto>>> ObtenerTodos(
        [FromQuery] string? cedula, CancellationToken ct)
    {
        var turnos = await _turnoService.ObtenerTodosAsync(ct);

        if (!string.IsNullOrWhiteSpace(cedula))
            turnos = turnos.Where(t => t.Cedula == cedula).ToList();

        return Ok(turnos);
    }

    [HttpPatch("{id:int}/activar")]
    public async Task<ActionResult<TurnoDto>> Activar(int id, CancellationToken ct)
    {
        var turno = await _turnoService.ActivarTurnoAsync(id, ct);
        return Ok(turno);
    }
}
