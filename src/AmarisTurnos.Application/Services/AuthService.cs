using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Domain.Exceptions;

namespace AmarisTurnos.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IUsuarioRepository usuarioRepository, IJwtTokenGenerator tokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponseDto> RegistrarAsync(RegistroDto dto, CancellationToken ct = default)
    {
        if (await _usuarioRepository.ExisteCedulaAsync(dto.Cedula, ct))
        {
            throw new ReglaDeNegocioException("Ya existe una cuenta registrada con esa cédula.");
        }

        var usuario = new Usuario
        {
            Cedula = dto.Cedula,
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = RolUsuario.Usuario, // el autoregistro SIEMPRE crea rol Usuario, nunca Admin
            Activo = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _usuarioRepository.CrearAsync(usuario, ct);

        var (token, expiraUtc) = _tokenGenerator.GenerarToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            ExpiraUtc = expiraUtc,
            Cedula = usuario.Cedula,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol.ToString()
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.ObtenerPorCedulaAsync(dto.Cedula, ct);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
        {
            throw new ReglaDeNegocioException("Cédula o contraseña incorrectos.");
        }

        if (!usuario.Activo)
        {
            throw new ReglaDeNegocioException("Esta cuenta está desactivada. Contacta al administrador.");
        }

        var (token, expiraUtc) = _tokenGenerator.GenerarToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            ExpiraUtc = expiraUtc,
            Cedula = usuario.Cedula,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol.ToString()
        };
    }
}