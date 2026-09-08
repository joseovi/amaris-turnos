using AmarisTurnos.Domain.Entities;

namespace AmarisTurnos.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default);
    }
}
