using System.ComponentModel.DataAnnotations;

namespace AmarisTurnos.Application.DTOs
{
    public class CrearTurnoDto
    {
        [Required, Range(1, int.MaxValue)]
        public int SucursalId { get; set; }
    }
}
