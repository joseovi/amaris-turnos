using System.ComponentModel.DataAnnotations;

namespace AmarisTurnos.Application.DTOs
{
    public class LoginDto
    {
        [Required]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
