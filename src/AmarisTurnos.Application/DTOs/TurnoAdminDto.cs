namespace AmarisTurnos.Application.DTOs
{
    public class TurnoAdminDto
    {
        public int Id { get; set; }
        public string UsuarioCedula { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string SucursalNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacionUtc { get; set; }
        public DateTime FechaExpiracionUtc { get; set; }
        public DateTime? FechaActivacionUtc { get; set; }
        public DateTime? FechaFinalizacionUtc { get; set; }
    }
}
