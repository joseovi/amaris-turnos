using System.ComponentModel.DataAnnotations;

namespace AmarisTurnos.Application.DTOs
{
    public class CrearUsuarioAdminDto
    {
        [Required, RegularExpression(@"^\d{6,15}$")]
        public string Cedula { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "Usuario";
    }
}
