using AmarisTurnos.Application.DTOs;

namespace AmarisTurnos.Application.Interfaces
{
    public interface IAdminTurnoService
    {
        Task<List<TurnoAdminDto>> ListarConFiltrosAsync(FiltroTurnosDto filtro, CancellationToken ct = default);
        Task<TurnoAdminDto> CancelarAsync(int turnoId, CancellationToken ct = default);
        Task<DashboardDto> ObtenerDashboardAsync(CancellationToken ct = default);
        Task<TurnoAdminDto> CompletarAsync(int turnoId, CancellationToken ct = default);
    }
}
