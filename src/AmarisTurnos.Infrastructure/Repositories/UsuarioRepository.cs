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
}