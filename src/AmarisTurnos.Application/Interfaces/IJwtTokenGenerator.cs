using AmarisTurnos.Domain.Entities;

namespace AmarisTurnos.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiraUtc) GenerarToken(Usuario usuario);
    }
}
