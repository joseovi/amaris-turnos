using System.ComponentModel.DataAnnotations;

namespace AmarisTurnos.Application.DTOs
{
    public class CrearSucursalDto
    {
        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Direccion { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Ciudad { get; set; } = string.Empty;
    }
}
