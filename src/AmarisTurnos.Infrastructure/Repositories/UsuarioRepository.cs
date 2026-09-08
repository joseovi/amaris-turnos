using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Infrastructure.Persitence;
using Microsoft.EntityFrameworkCore;

namespace AmarisTurnos.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly TurnosDbContext _context;

        public UsuarioRepository(TurnosDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
            => await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario, ct);
    }
}
