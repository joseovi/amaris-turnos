using AmarisTurnos.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace AmarisTurnos.Api.Middleware
{
    public class ManejoDeExcepcionesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejoDeExcepcionesMiddleware> _logger;

        public ManejoDeExcepcionesMiddleware(RequestDelegate next, ILogger<ManejoDeExcepcionesMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ReglaDeNegocioException ex)
            {
                await EscribirRespuestaAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado procesando {Path}", context.Request.Path);
                await EscribirRespuestaAsync(context, HttpStatusCode.InternalServerError,
                    "Ocurrió un error inesperado. Intenta nuevamente.");
            }
        }

        private static async Task EscribirRespuestaAsync(HttpContext context, HttpStatusCode statusCode, string mensaje)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var payload = JsonSerializer.Serialize(new { error = mensaje });
            await context.Response.WriteAsync(payload);
        }
    }
}
