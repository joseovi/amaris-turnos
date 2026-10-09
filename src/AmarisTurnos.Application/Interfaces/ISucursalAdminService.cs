using AmarisTurnos.Application.DTOs;

namespace AmarisTurnos.Application.Interfaces
{
    public interface ISucursalAdminService
    {
        Task<SucursalDto> CrearAsync(CrearSucursalDto dto, CancellationToken ct = default);
        Task<SucursalDto> EditarAsync(int id, EditarSucursalDto dto, CancellationToken ct = default);
        Task<SucursalDto> CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}
