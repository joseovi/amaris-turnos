using AmarisTurnos.Domain.Entities;

namespace AmarisTurnos.Application.Interfaces
{
    public interface ITurnoRepository
    {
        Task<(bool Creado, int TurnosHoy)> CrearConValidacionDeLimiteAsync(
        Turno turno,
        DateTime inicioDiaUtc,
        DateTime finDiaUtc,
        int limiteDiario,
        CancellationToken ct = default);

        Task<Turno?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

        Task<List<Turno>> ObtenerTodosAsync(CancellationToken ct = default);

        Task GuardarCambiosAsync(CancellationToken ct = default);
    }
}
