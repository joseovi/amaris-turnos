using Microsoft.EntityFrameworkCore;
using AmarisTurnos.Application.Interfaces;
using AmarisTurnos.Domain.Entities;
using AmarisTurnos.Infrastructure.Persistence;

namespace AmarisTurnos.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly TurnosDbContext _context;

    public UsuarioRepository(TurnosDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorCedulaAsync(string cedula, CancellationToken ct = default)
        => await _context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == cedula, ct);

    public async Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        => await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<bool> ExisteCedulaAsync(string cedula, CancellationToken ct = default)
        => await _context.Usuarios.AnyAsync(u => u.Cedula == cedula, ct);

    public async Task<Usuario> CrearAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(ct);
        return usuario;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync(string? busqueda, CancellationToken ct = default)
    {
        var query = _context.Usuarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            query = query.Where(u =>
                u.Cedula.Contains(busqueda) ||
                u.Nombre.Contains(busqueda));
        }

        return await query.OrderBy(u => u.Nombre).ToListAsync(ct);
    }

    public async Task GuardarCambiosAsync(CancellationToken ct = default)
    => await _context.SaveChangesAsync(ct);

    public async Task EliminarAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync(ct);
    }
}