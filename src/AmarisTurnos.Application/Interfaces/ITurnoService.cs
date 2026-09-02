using AmarisTurnos.Application.DTOs;

namespace AmarisTurnos.Application.Interfaces
{
    public interface ITurnoService
    {
        Task<TurnoDto> CrearTurnoAsync(CrearTurnoDto dto, CancellationToken ct = default);
        Task<TurnoDto> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<List<TurnoDto>> ObtenerTodosAsync(CancellationToken ct = default);
        Task<TurnoDto> ActivarTurnoAsync(int id, CancellationToken ct = default);
    }
}
