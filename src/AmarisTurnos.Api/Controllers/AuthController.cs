using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TurnosBanco.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var resultado = await _authService.LoginAsync(dto, ct);
        return Ok(resultado);
    }
}