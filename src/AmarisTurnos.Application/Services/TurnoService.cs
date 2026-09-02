using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Domain.Exceptions;

namespace AmarisTurnos.Application.Services
{
    public class TurnoService : ITurnoService
    {
        private const int LimiteTurnosDiarios = 5;
        private const int MinutosParaExpirar = 15;

        private readonly ITurnoRepository _turnoRepository;
        private readonly ISucursalRepository _sucursalRepository;

        public TurnoService(ITurnoRepository turnoRepository, ISucursalRepository sucursalRepository)
        {
            _turnoRepository = turnoRepository;
            _sucursalRepository = sucursalRepository;
        }

        public async Task<TurnoDto> CrearTurnoAsync(CrearTurnoDto dto, CancellationToken ct = default)
        {
            var sucursal = await _sucursalRepository.ObtenerPorIdAsync(dto.SucursalId, ct)
                ?? throw new ReglaDeNegocioException("La sucursal indicada no existe.");

            var ahoraUtc = DateTime.UtcNow;
            var inicioDiaUtc = ahoraUtc.Date;               // 00:00 UTC del día actual
            var finDiaUtc = inicioDiaUtc.AddDays(1);         // 00:00 UTC del día siguiente

            var turno = new Turno
            {
                Cedula = dto.Cedula,
                SucursalId = dto.SucursalId,
                Estado = EstadoTurno.Pending,
                FechaCreacionUtc = ahoraUtc,
                FechaExpiracionUtc = ahoraUtc.AddMinutes(MinutosParaExpirar)
            };

            var (creado, turnosHoy) = await _turnoRepository.CrearConValidacionDeLimiteAsync(
                turno, inicioDiaUtc, finDiaUtc, LimiteTurnosDiarios, ct);

            if (!creado)
            {
                throw new ReglaDeNegocioException(
                    $"La cédula {dto.Cedula} ya alcanzó el límite de {LimiteTurnosDiarios} turnos para hoy.");
            }

            return MapearADto(turno, sucursal.Nombre);
        }

        public async Task<TurnoDto> ActivarTurnoAsync(int id, CancellationToken ct = default)
        {
            var turno = await _turnoRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("El turno no existe.");

            if (turno.Estado == EstadoTurno.Expired ||
                (turno.Estado == EstadoTurno.Pending && DateTime.UtcNow > turno.FechaExpiracionUtc))
            {
                turno.Estado = EstadoTurno.Expired;
                await _turnoRepository.GuardarCambiosAsync(ct);
                throw new ReglaDeNegocioException("El turno ya expiró y no puede activarse.");
            }

            if (turno.Estado != EstadoTurno.Pending)
            {
                throw new ReglaDeNegocioException(
                    $"No se puede activar un turno en estado {turno.Estado}.");
            }

            turno.Estado = EstadoTurno.Active;
            turno.FechaActivacionUtc = DateTime.UtcNow;
            await _turnoRepository.GuardarCambiosAsync(ct);

            return MapearADto(turno, turno.Sucursal?.Nombre ?? string.Empty);
        }

        public async Task<TurnoDto> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            var turno = await _turnoRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("El turno no existe.");

            return MapearADto(turno, turno.Sucursal?.Nombre ?? string.Empty);
        }

        public async Task<List<TurnoDto>> ObtenerTodosAsync(CancellationToken ct = default)
        {
            var turnos = await _turnoRepository.ObtenerTodosAsync(ct);
            return turnos.Select(t => MapearADto(t, t.Sucursal?.Nombre ?? string.Empty)).ToList();
        }

        private static TurnoDto MapearADto(Turno turno, string sucursalNombre) => new()
        {
            Id = turno.Id,
            Cedula = turno.Cedula,
            SucursalId = turno.SucursalId,
            SucursalNombre = sucursalNombre,
            Estado = turno.Estado.ToString(),
            FechaCreacionUtc = turno.FechaCreacionUtc,
            FechaExpiracionUtc = turno.FechaExpiracionUtc,
            FechaActivacionUtc = turno.FechaActivacionUtc,
            FechaFinalizacionUtc = turno.FechaFinalizacionUtc
        };
    }
}
