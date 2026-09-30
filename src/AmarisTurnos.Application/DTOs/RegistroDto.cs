using System.ComponentModel.DataAnnotations;

namespace AmarisTurnos.Application.DTOs
{
    public class RegistroDto
    {
        [Required, RegularExpression(@"^\d{6,15}$", ErrorMessage = "Cédula inválida.")]
        public string Cedula { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Required, MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
