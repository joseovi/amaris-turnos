using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;

namespace AmarisTurnos.Api.Controllers;

[ApiController]
[Route("api/admin/usuarios")]
[Authorize(Roles = "Admin")] // protege TODO el controller, ningún endpoint aquí es accesible sin rol Admin
public class UsuariosAdminController : ControllerBase
{
    private readonly IUsuarioAdminService _usuarioAdminService;

    public UsuariosAdminController(IUsuarioAdminService usuarioAdminService)
    {
        _usuarioAdminService = usuarioAdminService;
    }

    private int UsuarioIdActual =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<UsuarioAdminDto>>> Listar([FromQuery] string? busqueda, CancellationToken ct)
    {
        var usuarios = await _usuarioAdminService.ListarAsync(busqueda, ct);
        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioAdminDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var usuario = await _usuarioAdminService.ObtenerPorIdAsync(id, ct);
        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioAdminDto>> Crear([FromBody] CrearUsuarioAdminDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioAdminService.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioAdminDto>> Editar(int id, [FromBody] EditarUsuarioDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioAdminService.EditarAsync(id, dto, ct);
        return Ok(usuario);
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<ActionResult<UsuarioAdminDto>> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioAdminService.CambiarEstadoAsync(id, dto.Activo, UsuarioIdActual, ct);
        return Ok(usuario);
    }

    [HttpPatch("{id:int}/rol")]
    public async Task<ActionResult<UsuarioAdminDto>> CambiarRol(int id, [FromBody] CambiarRolDto dto, CancellationToken ct)
    {
        var usuario = await _usuarioAdminService.CambiarRolAsync(id, dto.Rol, UsuarioIdActual, ct);
        return Ok(usuario);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await _usuarioAdminService.EliminarAsync(id, UsuarioIdActual, ct);
        return NoContent();
    }
}