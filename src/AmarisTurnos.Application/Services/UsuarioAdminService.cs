using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AmarisTurnos.Application.Services
{
    public class UsuarioAdminService : IUsuarioAdminService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioAdminService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioAdminDto>> ListarAsync(string? busqueda, CancellationToken ct = default)
        {
            var usuarios = await _usuarioRepository.ObtenerTodosAsync(busqueda, ct);
            return usuarios.Select(MapearADto).ToList();
        }

        public async Task<UsuarioAdminDto> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("El usuario no existe.");
            return MapearADto(usuario);
        }

        public async Task<UsuarioAdminDto> CrearAsync(CrearUsuarioAdminDto dto, CancellationToken ct = default)
        {
            if (await _usuarioRepository.ExisteCedulaAsync(dto.Cedula, ct))
                throw new ReglaDeNegocioException("Ya existe un usuario con esa cédula.");

            if (!Enum.TryParse<RolUsuario>(dto.Rol, ignoreCase: true, out var rol))
                throw new ReglaDeNegocioException("Rol inválido. Debe ser 'Usuario' o 'Admin'.");

            var usuario = new Usuario
            {
                Cedula = dto.Cedula,
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = rol,
                Activo = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            try
            {
                await _usuarioRepository.CrearAsync(usuario, ct);
            }
            catch (DbUpdateException)
            {
                throw new ReglaDeNegocioException("Ya existe un usuario con esa cédula.");
            }

            return MapearADto(usuario);
        }

        public async Task<UsuarioAdminDto> EditarAsync(int id, EditarUsuarioDto dto, CancellationToken ct = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("El usuario no existe.");

            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;

            await _usuarioRepository.GuardarCambiosAsync(ct);
            return MapearADto(usuario);
        }

        public async Task<UsuarioAdminDto> CambiarEstadoAsync(int id, bool activo, int usuarioQueEjecutaId, CancellationToken ct = default)
        {
            if (id == usuarioQueEjecutaId && !activo)
                throw new ReglaDeNegocioException("No puedes desactivar tu propia cuenta.");

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("El usuario no existe.");

            usuario.Activo = activo;
            await _usuarioRepository.GuardarCambiosAsync(ct);
            return MapearADto(usuario);
        }

        public async Task<UsuarioAdminDto> CambiarRolAsync(int id, string nuevoRol, int usuarioQueEjecutaId, CancellationToken ct = default)
        {
            if (id == usuarioQueEjecutaId)
                throw new ReglaDeNegocioException("No puedes cambiar tu propio rol.");

            if (!Enum.TryParse<RolUsuario>(nuevoRol, ignoreCase: true, out var rol))
                throw new ReglaDeNegocioException("Rol inválido. Debe ser 'Usuario' o 'Admin'.");

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("El usuario no existe.");

            usuario.Rol = rol;
            await _usuarioRepository.GuardarCambiosAsync(ct);
            return MapearADto(usuario);
        }

        public async Task EliminarAsync(int id, int usuarioQueEjecutaId, CancellationToken ct = default)
        {
            if (id == usuarioQueEjecutaId)
                throw new ReglaDeNegocioException("No puedes eliminar tu propia cuenta.");

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id, ct)
                ?? throw new ReglaDeNegocioException("El usuario no existe.");

            try
            {
                await _usuarioRepository.EliminarAsync(usuario, ct);
            }
            catch (DbUpdateException)
            {
                throw new ReglaDeNegocioException(
                    "No se puede eliminar un usuario con turnos asociados. Desactívalo en su lugar.");
            }
        }

        private static UsuarioAdminDto MapearADto(Usuario u) => new()
        {
            Id = u.Id,
            Cedula = u.Cedula,
            Nombre = u.Nombre,
            Email = u.Email,
            Rol = u.Rol.ToString(),
            Activo = u.Activo,
            CreatedAtUtc = u.CreatedAtUtc
        };
    }
}
