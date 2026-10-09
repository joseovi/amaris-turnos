using AmarisTurnos.Application.DTOs;

namespace AmarisTurnos.Application.Interfaces
{
    public interface IUsuarioAdminService
    {
        Task<List<UsuarioAdminDto>> ListarAsync(string? busqueda, CancellationToken ct = default);
        Task<UsuarioAdminDto> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<UsuarioAdminDto> CrearAsync(CrearUsuarioAdminDto dto, CancellationToken ct = default);
        Task<UsuarioAdminDto> EditarAsync(int id, EditarUsuarioDto dto, CancellationToken ct = default);
        Task<UsuarioAdminDto> CambiarEstadoAsync(int id, bool activo, int usuarioQueEjecutaId, CancellationToken ct = default);
        Task<UsuarioAdminDto> CambiarRolAsync(int id, string nuevoRol, int usuarioQueEjecutaId, CancellationToken ct = default);
        Task EliminarAsync(int id, int usuarioQueEjecutaId, CancellationToken ct = default);
    }
}
