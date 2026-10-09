using System.ComponentModel.DataAnnotations;

namespace AmarisTurnos.Application.DTOs
{
    public class EditarUsuarioDto
    {
        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }
    }
}
