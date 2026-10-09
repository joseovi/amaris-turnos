namespace AmarisTurnos.Application.DTOs
{
    public class DashboardDto
    {
        public int TurnosHoy { get; set; }
        public int Pendientes { get; set; }
        public int Activos { get; set; }
        public int Completados { get; set; }
        public int Expirados { get; set; }
        public int Cancelados { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalSucursales { get; set; }
    }
}
