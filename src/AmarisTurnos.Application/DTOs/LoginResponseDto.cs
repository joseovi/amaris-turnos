namespace AmarisTurnos.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraUtc { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
