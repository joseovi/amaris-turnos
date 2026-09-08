using System.ComponentModel.DataAnnotations;

namespace AmarisTurnos.Application.DTOs
{
    public class CrearTurnoDto
    {
        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [RegularExpression(@"^\d{6,15}$", ErrorMessage = "La cédula debe contener solo números (6 a 15 dígitos).")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe indicar la sucursal.")]
        [Range(1, int.MaxValue, ErrorMessage = "La sucursal indicada no es válida.")]
        public int SucursalId { get; set; }
    }
}
