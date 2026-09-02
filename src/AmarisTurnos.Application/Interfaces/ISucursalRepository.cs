using AmarisTurnos.Domain.Entities;

namespace AmarisTurnos.Application.Interfaces
{
    public interface ISucursalRepository
    {
        Task<Sucursal?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<List<Sucursal>> ObtenerTodasAsync(CancellationToken ct = default);
        Task<Sucursal> CrearAsync(Sucursal sucursal, CancellationToken ct = default);
    }
}
