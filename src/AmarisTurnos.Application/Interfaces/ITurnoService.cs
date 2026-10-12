using AmarisTurnos.Application.DTOs;

namespace AmarisTurnos.Application.Interfaces
{
    public interface ITurnoService
    {
        Task<TurnoDto> CrearTurnoAsync(int UsuarioId, CrearTurnoDto dto, CancellationToken ct = default);
        Task<TurnoDto> ObtenerPorIdAsync(int usuarioId, int id, CancellationToken ct = default);
        Task<List<TurnoDto>> ObtenerMisTurnosDeHoyAsync(int usuarioId, CancellationToken ct = default);
        Task<TurnoDto> ActivarTurnoAsync(int usuarioId, int turnoId, CancellationToken ct = default);
        Task<int> ExpirarTurnosVencidosAsync(CancellationToken ct = default);
    }
}
