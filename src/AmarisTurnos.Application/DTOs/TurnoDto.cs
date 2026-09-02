namespace AmarisTurnos.Application.DTOs
{
    public class TurnoDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacionUtc { get; set; }
        public DateTime FechaExpiracionUtc { get; set; }
        public DateTime? FechaActivacionUtc { get; set; }
        public DateTime? FechaFinalizacionUtc { get; set; }
    }
}
