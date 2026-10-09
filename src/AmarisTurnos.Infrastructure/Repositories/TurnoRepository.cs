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
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id, ct);

        public async Task<List<Turno>> ObtenerTodosAsync(CancellationToken ct = default)
            => await _context.Turnos
                .Include(t => t.Sucursal)
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.FechaCreacionUtc)
                .ToListAsync(ct);

        public async Task<List<Turno>> ObtenerPorUsuarioYFechaAsync(int usuarioId, DateTime fechaUtc, CancellationToken ct = default)
        {
            var inicioDia = fechaUtc.Date;
            var finDia = inicioDia.AddDays(1);

            return await _context.Turnos
                .Include(t => t.Sucursal)
                .Include(t => t.Usuario)
                .Where(t => t.UsuarioId == usuarioId && t.FechaCreacionUtc >= inicioDia && t.FechaCreacionUtc < finDia)
                .OrderByDescending(t => t.FechaCreacionUtc)
                .ToListAsync(ct);
        }

        public async Task<List<Turno>> ObtenerConFiltrosAsync(
                DateTime? fecha, int? sucursalId, EstadoTurno? estado, string? cedula, CancellationToken ct = default)
        {
            var query = _context.Turnos
                .Include(t => t.Sucursal)
                .Include(t => t.Usuario)
                .AsQueryable();

            if (fecha.HasValue)
            {
                var inicio = fecha.Value.Date;
                var fin = inicio.AddDays(1);
                query = query.Where(t => t.FechaCreacionUtc >= inicio && t.FechaCreacionUtc < fin);
            }

            if (sucursalId.HasValue)
                query = query.Where(t => t.SucursalId == sucursalId.Value);

            if (estado.HasValue)
                query = query.Where(t => t.Estado == estado.Value);

            if (!string.IsNullOrWhiteSpace(cedula))
                query = query.Where(t => t.Usuario!.Cedula.Contains(cedula));

            return await query.OrderByDescending(t => t.FechaCreacionUtc).ToListAsync(ct);
        }

        public async Task<Dictionary<EstadoTurno, int>> ContarPorEstadoHoyAsync(CancellationToken ct = default)
        {
            var inicio = DateTime.UtcNow.Date;
            var fin = inicio.AddDays(1);

            return await _context.Turnos
                .Where(t => t.FechaCreacionUtc >= inicio && t.FechaCreacionUtc < fin)
                .GroupBy(t => t.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(x => x.Estado, x => x.Cantidad, ct);
        }

        public async Task GuardarCambiosAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public async Task<int> ExpirarVencidosAsync(DateTime ahoraUtc, CancellationToken ct = default)
            => await _context.Turnos
               .Where(t => t.Estado == EstadoTurno.Pending && t.FechaExpiracionUtc < ahoraUtc)
               .ExecuteUpdateAsync(s => s.SetProperty(t => t.Estado, EstadoTurno.Expired), ct);
    }

}
    
