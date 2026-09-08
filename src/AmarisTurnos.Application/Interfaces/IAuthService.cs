using AmarisTurnos.Application.DTOs;

namespace AmarisTurnos.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    }
}
