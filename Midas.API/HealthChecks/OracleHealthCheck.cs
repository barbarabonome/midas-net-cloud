using Microsoft.Extensions.Diagnostics.HealthChecks;
using Midas.Infrastructure.Persistence;

namespace Midas.API.HealthChecks
{
    public class OracleHealthCheck : IHealthCheck
    {
        private readonly MidasContext _context;
        private readonly ILogger<OracleHealthCheck> _logger;

        public OracleHealthCheck(MidasContext context, ILogger<OracleHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Tenta verificar a conexão com o banco de dados Oracle
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

                if (!canConnect)
                {
                    _logger.LogError("Falha ao conectar com o banco de dados Oracle");
                    return HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados Oracle");
                }

                _logger.LogInformation("Conexão com Oracle estabelecida com sucesso");
                return HealthCheckResult.Healthy("Conexão com Oracle estabelecida com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar saúde do banco de dados Oracle");
                return HealthCheckResult.Unhealthy($"Erro ao conectar com o Oracle: {ex.Message}");
            }
        }
    }
}
