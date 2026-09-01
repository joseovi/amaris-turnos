namespace AmarisTurnos.Domain.Entities
{
    public class Turno
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
        public EstadoTurno Estado { get; set; } = EstadoTurno.Pending;

        public DateTime FechaCreacionUtc { get; set; }
        public DateTime FechaExpiracionUtc { get; set; }
        public DateTime? FechaActivacionUtc { get; set; }
        public DateTime? FechaFinalizacionUtc { get; set; }
    }
}
