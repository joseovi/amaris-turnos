using AmarisTurnos.Domain.Entities;

namespace AmarisTurnos.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorCedulaAsync(string cedula, CancellationToken ct = default);
        Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<bool> ExisteCedulaAsync(string cedula, CancellationToken ct = default);
        Task<Usuario> CrearAsync(Usuario usuario, CancellationToken ct = default);
        Task<List<Usuario>> ObtenerTodosAsync(string? busqueda, CancellationToken ct = default);
        Task GuardarCambiosAsync(CancellationToken ct = default);
        Task EliminarAsync(Usuario usuario, CancellationToken ct = default);
    }
}
