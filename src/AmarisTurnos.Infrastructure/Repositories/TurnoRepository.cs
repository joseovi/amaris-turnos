using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AmarisTurnos.Infrastructure.Repositories
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly TurnosDbContext _context;

        public TurnoRepository(TurnosDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Creado, int TurnosHoy)> CrearConValidacionDeLimiteAsync(
            Turno turno,
            DateTime inicioDiaUtc,
            DateTime finDiaUtc,
            int limiteDiario,
            CancellationToken ct = default)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            try
            {
                var turnosHoy = await _context.Turnos
                .FromSqlInterpolated($@"
                    SELECT * FROM Turnos WITH (UPDLOCK, HOLDLOCK)
                    WHERE UsuarioId = {turno.UsuarioId}
                      AND FechaCreacionUtc >= {inicioDiaUtc}
                      AND FechaCreacionUtc < {finDiaUtc}")
                .CountAsync(ct);

                if (turnosHoy >= limiteDiario)
                {
                    await transaction.RollbackAsync(ct);
                    return (false, turnosHoy);
                }

                _context.Turnos.Add(turno);
                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return (true, turnosHoy + 1);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<Turno?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => await _context.Turnos
                .Include(t => t.Sucursal)
                .FirstOrDefaultAsync(t => t.Id == id, ct);

        public async Task<List<Turno>> ObtenerTodosAsync(CancellationToken ct = default)
            => await _context.Turnos
                .Include(t => t.Sucursal)
                .OrderByDescending(t => t.FechaCreacionUtc)
                .ToListAsync(ct);

        public async Task<List<Turno>> ObtenerPorUsuarioYFechaAsync(int usuarioId, DateTime fechaUtc, CancellationToken ct = default)
        {
            var inicioDia = fechaUtc.Date;
            var finDia = inicioDia.AddDays(1);

            return await _context.Turnos
                .Include(t => t.Sucursal)
                .Where(t => t.UsuarioId == usuarioId && t.FechaCreacionUtc >= inicioDia && t.FechaCreacionUtc < finDia)
                .OrderByDescending(t => t.FechaCreacionUtc)
                .ToListAsync(ct);
        }

        public async Task GuardarCambiosAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }

}
    
