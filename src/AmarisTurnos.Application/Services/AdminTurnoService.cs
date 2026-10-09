using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Domain.Exceptions;

namespace AmarisTurnos.Application.Services
{
    public class AdminTurnoService : IAdminTurnoService
    {
        private readonly ITurnoRepository _turnoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISucursalRepository _sucursalRepository;

        public AdminTurnoService(
            ITurnoRepository turnoRepository,
            IUsuarioRepository usuarioRepository,
            ISucursalRepository sucursalRepository)
        {
            _turnoRepository = turnoRepository;
            _usuarioRepository = usuarioRepository;
            _sucursalRepository = sucursalRepository;
        }

        public async Task<List<TurnoAdminDto>> ListarConFiltrosAsync(FiltroTurnosDto filtro, CancellationToken ct = default)
        {
            EstadoTurno? estado = null;
            if (!string.IsNullOrWhiteSpace(filtro.Estado) &&
                Enum.TryParse<EstadoTurno>(filtro.Estado, ignoreCase: true, out var parsed))
            {
                estado = parsed;
            }

            var turnos = await _turnoRepository.ObtenerConFiltrosAsync(
                filtro.Fecha, filtro.SucursalId, estado, filtro.Cedula, ct);

            return turnos.Select(t => new TurnoAdminDto
            {
                Id = t.Id,
                UsuarioCedula = t.Usuario?.Cedula ?? string.Empty,
                UsuarioNombre = t.Usuario?.Nombre ?? string.Empty,
                SucursalNombre = t.Sucursal?.Nombre ?? string.Empty,
                Estado = t.Estado.ToString(),
                FechaCreacionUtc = t.FechaCreacionUtc,
                FechaExpiracionUtc = t.FechaExpiracionUtc,
                FechaActivacionUtc = t.FechaActivacionUtc,
                FechaFinalizacionUtc = t.FechaFinalizacionUtc
            }).ToList();
        }
        public async Task<TurnoAdminDto> CompletarAsync(int turnoId, CancellationToken ct = default)
        {
            var turno = await _turnoRepository.ObtenerPorIdAsync(turnoId, ct)
                ?? throw new ReglaDeNegocioException("El turno no existe.");

            if (turno.Estado != EstadoTurno.Active)
            {
                throw new ReglaDeNegocioException(
                    $"Solo se puede completar un turno que esté Activo. Estado actual: {turno.Estado}.");
            }

            turno.Estado = EstadoTurno.Completed;
            turno.FechaFinalizacionUtc = DateTime.UtcNow;
            await _turnoRepository.GuardarCambiosAsync(ct);

            return new TurnoAdminDto
            {
                Id = turno.Id,
                UsuarioCedula = turno.Usuario?.Cedula ?? string.Empty,
                UsuarioNombre = turno.Usuario?.Nombre ?? string.Empty,
                SucursalNombre = turno.Sucursal?.Nombre ?? string.Empty,
                Estado = turno.Estado.ToString(),
                FechaCreacionUtc = turno.FechaCreacionUtc,
                FechaExpiracionUtc = turno.FechaExpiracionUtc,
                FechaActivacionUtc = turno.FechaActivacionUtc,
                FechaFinalizacionUtc = turno.FechaFinalizacionUtc
            };
        }

        public async Task<TurnoAdminDto> CancelarAsync(int turnoId, CancellationToken ct = default)
        {
            var turno = await _turnoRepository.ObtenerPorIdAsync(turnoId, ct)
                ?? throw new ReglaDeNegocioException("El turno no existe.");

            if (turno.Estado is EstadoTurno.Completed or EstadoTurno.Cancelled or EstadoTurno.Expired)
            {
                throw new ReglaDeNegocioException(
                    $"No se puede cancelar un turno en estado {turno.Estado}.");
            }

            turno.Estado = EstadoTurno.Cancelled;
            turno.FechaFinalizacionUtc = DateTime.UtcNow;
            await _turnoRepository.GuardarCambiosAsync(ct);

            return new TurnoAdminDto
            {
                Id = turno.Id,
                UsuarioCedula = turno.Usuario?.Cedula ?? string.Empty,
                UsuarioNombre = turno.Usuario?.Nombre ?? string.Empty,
                SucursalNombre = turno.Sucursal?.Nombre ?? string.Empty,
                Estado = turno.Estado.ToString(),
                FechaCreacionUtc = turno.FechaCreacionUtc,
                FechaExpiracionUtc = turno.FechaExpiracionUtc,
                FechaActivacionUtc = turno.FechaActivacionUtc,
                FechaFinalizacionUtc = turno.FechaFinalizacionUtc
            };
        }

        public async Task<DashboardDto> ObtenerDashboardAsync(CancellationToken ct = default)
        {
            var conteos = await _turnoRepository.ContarPorEstadoHoyAsync(ct);
            var usuarios = await _usuarioRepository.ObtenerTodosAsync(null, ct);
            var sucursales = await _sucursalRepository.ObtenerTodasAsync(ct);

            int ObtenerConteo(EstadoTurno estado) => conteos.GetValueOrDefault(estado, 0);

            return new DashboardDto
            {
                TurnosHoy = conteos.Values.Sum(),
                Pendientes = ObtenerConteo(EstadoTurno.Pending),
                Activos = ObtenerConteo(EstadoTurno.Active),
                Completados = ObtenerConteo(EstadoTurno.Completed),
                Expirados = ObtenerConteo(EstadoTurno.Expired),
                Cancelados = ObtenerConteo(EstadoTurno.Cancelled),
                TotalUsuarios = usuarios.Count,
                TotalSucursales = sucursales.Count
            };
        }
    }
}
