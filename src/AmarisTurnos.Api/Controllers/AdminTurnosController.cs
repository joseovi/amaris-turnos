using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmarisTurnos.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminTurnosController : ControllerBase
    {
        private readonly IAdminTurnoService _adminTurnoService;

        public AdminTurnosController(IAdminTurnoService adminTurnoService)
        {
            _adminTurnoService = adminTurnoService;
        }

        [HttpGet("turnos")]
        public async Task<ActionResult<List<TurnoAdminDto>>> Listar([FromQuery] FiltroTurnosDto filtro, CancellationToken ct)
        {
            var turnos = await _adminTurnoService.ListarConFiltrosAsync(filtro, ct);
            return Ok(turnos);
        }

        [HttpPatch("turnos/{id:int}/cancelar")]
        public async Task<ActionResult<TurnoAdminDto>> Cancelar(int id, CancellationToken ct)
        {
            var turno = await _adminTurnoService.CancelarAsync(id, ct);
            return Ok(turno);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> Dashboard(CancellationToken ct)
        {
            var dashboard = await _adminTurnoService.ObtenerDashboardAsync(ct);
            return Ok(dashboard);
        }

        [HttpPatch("turnos/{id:int}/completar")]
        public async Task<ActionResult<TurnoAdminDto>> Completar(int id, CancellationToken ct)
        {
            var turno = await _adminTurnoService.CompletarAsync(id, ct);
            return Ok(turno);
        }
    }
}

