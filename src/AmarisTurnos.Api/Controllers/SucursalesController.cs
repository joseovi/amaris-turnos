using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;

namespace AmarisTurnos.Api.Controllers;

[ApiController]
[Route("api/sucursales")]
[Authorize]
public class SucursalesController : ControllerBase
{
    private readonly ISucursalRepository _sucursalRepository;
    private readonly ISucursalAdminService _sucursalAdminService;

    public SucursalesController(
        ISucursalRepository sucursalRepository,
        ISucursalAdminService sucursalAdminService)
    {
        _sucursalRepository = sucursalRepository;
        _sucursalAdminService = sucursalAdminService;
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
            Ciudad = s.Ciudad,
            Activo = s.Activo
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SucursalDto>> Crear([FromBody] CrearSucursalDto dto, CancellationToken ct)
    {
        var sucursal = await _sucursalAdminService.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerTodas), sucursal);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SucursalDto>> Editar(int id, [FromBody] EditarSucursalDto dto, CancellationToken ct)
    {
        var sucursal = await _sucursalAdminService.EditarAsync(id, dto, ct);
        return Ok(sucursal);
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SucursalDto>> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto, CancellationToken ct)
    {
        var sucursal = await _sucursalAdminService.CambiarEstadoAsync(id, dto.Activo, ct);
        return Ok(sucursal);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await _sucursalAdminService.EliminarAsync(id, ct);
        return NoContent();
    }
}