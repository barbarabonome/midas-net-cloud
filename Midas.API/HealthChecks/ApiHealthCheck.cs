using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Midas.API.HealthChecks
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly ILogger<ApiHealthCheck> _logger;

        public ApiHealthCheck(ILogger<ApiHealthCheck> logger)
        {
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Verificando saúde geral da API");
                return Task.FromResult(HealthCheckResult.Healthy("API está saudável"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar saúde da API");
                return Task.FromResult(HealthCheckResult.Unhealthy($"Erro na API: {ex.Message}"));
            }
        }
    }
}
