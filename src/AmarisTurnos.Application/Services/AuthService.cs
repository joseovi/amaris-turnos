using AmarisTurnos.Application.DTOs;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Exceptions;

namespace AmarisTurnos.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthService(IUsuarioRepository usuarioRepository, IJwtTokenGenerator tokenGenerator)
        {
            _usuarioRepository = usuarioRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(dto.NombreUsuario, ct);

            if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                throw new ReglaDeNegocioException("Usuario o contraseña incorrectos.");
            }

            var (token, expiraUtc) = _tokenGenerator.GenerarToken(usuario);

            return new LoginResponseDto
            {
                Token = token,
                ExpiraUtc = expiraUtc,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol
            };
        }
    }
}
