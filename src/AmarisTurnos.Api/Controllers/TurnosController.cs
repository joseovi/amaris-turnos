using System.Security.Claims;
using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TurnosBanco.Api.Controllers;

[ApiController]
[Route("api/turnos")]
[Authorize]
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _turnoService;

    public TurnosController(ITurnoService turnoService)
    {
        _turnoService = turnoService;
    }

    private int UsuarioIdActual =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<TurnoDto>> Crear([FromBody] CrearTurnoDto dto, CancellationToken ct)
    {
        var turno = await _turnoService.CrearTurnoAsync(UsuarioIdActual, dto, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = turno.Id }, turno);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TurnoDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var turno = await _turnoService.ObtenerPorIdAsync(UsuarioIdActual, id, ct);
        return Ok(turno);
    }

    [HttpGet("mios")]
    public async Task<ActionResult<List<TurnoDto>>> ObtenerMisTurnosDeHoy(CancellationToken ct)
    {
        var turnos = await _turnoService.ObtenerMisTurnosDeHoyAsync(UsuarioIdActual, ct);
        return Ok(turnos);
    }

    [HttpPatch("{id:int}/activar")]
    public async Task<ActionResult<TurnoDto>> Activar(int id, CancellationToken ct)
    {
        var turno = await _turnoService.ActivarTurnoAsync(UsuarioIdActual, id, ct);
        return Ok(turno);
    }
}