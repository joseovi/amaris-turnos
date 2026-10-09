using AmarisTurnos.Application.Interfaces;

namespace AmarisTurnos.Api.BackgroundServices
{
    public class TurnoExpiracionWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TurnoExpiracionWorker> _logger;
        private readonly TimeSpan _intervalo;

        public TurnoExpiracionWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<TurnoExpiracionWorker> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _intervalo = TimeSpan.FromSeconds(
                configuration.GetValue("Turnos:IntervaloExpiracionSegundos", 60));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Una pasada inmediata al arrancar: recupera turnos que vencieron mientras la API estaba apagada
            await ExpirarVencidosAsync(stoppingToken);

            using var timer = new PeriodicTimer(_intervalo);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ExpirarVencidosAsync(stoppingToken);
            }
        }

        private async Task ExpirarVencidosAsync(CancellationToken ct)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var turnoService = scope.ServiceProvider.GetRequiredService<ITurnoService>();

                var expirados = await turnoService.ExpirarTurnosVencidosAsync(ct);

                if (expirados > 0)
                    _logger.LogInformation("Se expiraron {Cantidad} turno(s) vencido(s).", expirados);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error al expirar turnos vencidos. Se reintentará en el próximo ciclo.");
            }
        }
    }
}
