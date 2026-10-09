namespace AmarisTurnos.Domain.Entities;

public enum RolUsuario
{
    Usuario = 0,
    Admin= 1
}

    public class Usuario
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; } = RolUsuario.Usuario;
    public bool Activo { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}

