using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Infrastructure.Persitence;
using Microsoft.EntityFrameworkCore;

namespace AmarisTurnos.Infrastructure.Repositories
{
    public class SucursalRepository : ISucursalRepository
    {
        private readonly TurnosDbContext _context;

        public SucursalRepository(TurnosDbContext context)
        {
            _context = context;
        }

        public async Task<Sucursal?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => await _context.Sucursales.FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task<List<Sucursal>> ObtenerTodasAsync(CancellationToken ct = default)
            => await _context.Sucursales
                .OrderBy(s => s.Nombre)
                .ToListAsync(ct);

        public async Task<Sucursal> CrearAsync(Sucursal sucursal, CancellationToken ct = default)
        {
            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync(ct);
            return sucursal;
        }
    }
}
