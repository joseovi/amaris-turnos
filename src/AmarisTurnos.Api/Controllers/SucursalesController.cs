using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmarisTurnos.Api.Controllers;

[ApiController]
[Route("api/sucursales")]
[Authorize]
public class SucursalesController : ControllerBase
    {
    private readonly ISucursalRepository _sucursalRepository;

    public SucursalesController(ISucursalRepository sucursalRepository)
    {
        _sucursalRepository = sucursalRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<SucursalDto>>> ObtenerTodas(CancellationToken ct)
    {
        var sucursales = await _sucursalRepository.ObtenerTodasAsync(ct);

        var dtos = sucursales.Select(s => new SucursalDto
        {
            Id = s.Id,
            Nombre = s.Nombre,
            Direccion = s.Direccion,
            Ciudad = s.Ciudad
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] 
    public async Task<ActionResult<SucursalDto>> Crear([FromBody] CrearSucursalDto dto, CancellationToken ct)
    {
        var sucursal = new Sucursal
        {
            Nombre = dto.Nombre,
            Direccion = dto.Direccion,
            Ciudad = dto.Ciudad
        };

        var creada = await _sucursalRepository.CrearAsync(sucursal, ct);

        var resultado = new SucursalDto
        {
            Id = creada.Id,
            Nombre = creada.Nombre,
            Direccion = creada.Direccion,
            Ciudad = creada.Ciudad
        };

        return CreatedAtAction(nameof(ObtenerTodas), resultado);
    }
}

