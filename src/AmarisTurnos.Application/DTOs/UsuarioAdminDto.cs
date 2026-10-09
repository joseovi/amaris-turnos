namespace AmarisTurnos.Application.DTOs
{
    public class UsuarioAdminDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
