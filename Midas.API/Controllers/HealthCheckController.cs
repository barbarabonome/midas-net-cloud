using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Midas.API.HealthChecks;
using Midas.API.DTOs;

namespace Midas.API.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthCheckController> _logger;
        private readonly OracleHealthCheck _oracleHealthCheck;
        private readonly ApiHealthCheck _apiHealthCheck;

        public HealthCheckController(
            HealthCheckService healthCheckService,
            ILogger<HealthCheckController> logger,
            OracleHealthCheck oracleHealthCheck,
            ApiHealthCheck apiHealthCheck)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
            _oracleHealthCheck = oracleHealthCheck;
            _apiHealthCheck = apiHealthCheck;
        }

        /// <summary>
        /// Verifica a saúde geral da API
        /// </summary>
        /// <returns>Status geral da API</returns>
        [HttpGet("api")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<HealthCheckDetailResponse>> CheckApi()
        {
            _logger.LogInformation("Verificando saúde da API");
            var context = new HealthCheckContext();
            var result = await _apiHealthCheck.CheckHealthAsync(context);

            return CreateResponse("API", result);
        }

        /// <summary>
        /// Verifica a conectividade com o banco de dados Oracle
        /// </summary>
        /// <returns>Status de conexão do Oracle</returns>
        [HttpGet("database")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<HealthCheckDetailResponse>> CheckDatabase()
        {
            _logger.LogInformation("Verificando conectividade com o banco de dados");
            var context = new HealthCheckContext();
            var result = await _oracleHealthCheck.CheckHealthAsync(context);

            return CreateResponse("Oracle Database", result);
        }

        /// <summary>
        /// Verifica a saúde completa do sistema (API e Database)
        /// </summary>
        /// <returns>Status completo de todos os componentes</returns>
        [HttpGet("complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<CompleteHealthCheckResponse>> CheckComplete()
        {
            _logger.LogInformation("Verificando saúde completa do sistema");
            var context = new HealthCheckContext();

            var apiResult = await _apiHealthCheck.CheckHealthAsync(context);
            var databaseResult = await _oracleHealthCheck.CheckHealthAsync(context);

            var response = new CompleteHealthCheckResponse
            {
                Status = DetermineOverallStatus(apiResult.Status, databaseResult.Status),
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                ApiHealth = MapHealthResult("API", apiResult),
                DatabaseHealth = MapHealthResult("Oracle Database", databaseResult),
                Uptime = GetUptime()
            };

            var statusCode = response.Status == "Healthy" ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            return StatusCode(statusCode, response);
        }

        /// <summary>
        /// Verifica a saúde com informações resumidas
        /// </summary>
        /// <returns>Status resumido do sistema</returns>
        [HttpGet("ready")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ReadinessCheckResponse>> CheckReadiness()
        {
            _logger.LogInformation("Verificando disponibilidade do sistema");
            var context = new HealthCheckContext();

            var databaseResult = await _oracleHealthCheck.CheckHealthAsync(context);
            var isReady = databaseResult.Status == HealthStatus.Healthy;

            var response = new ReadinessCheckResponse
            {
                IsReady = isReady,
                Timestamp = DateTime.UtcNow,
                Details = new Dictionary<string, string>
                {
                    { "Database", databaseResult.Status.ToString() }
                }
            };

            return isReady ? Ok(response) : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        private ActionResult<HealthCheckDetailResponse> CreateResponse(string name, HealthCheckResult result)
        {
            var response = MapHealthResult(name, result);
            var statusCode = result.Status == HealthStatus.Healthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            return StatusCode(statusCode, response);
        }

        private HealthCheckDetailResponse MapHealthResult(string name, HealthCheckResult result)
        {
            return new HealthCheckDetailResponse
            {
                Name = name,
                Status = result.Status.ToString(),
                Description = result.Description ?? "Sem descrição",
                Duration = 0,
                Timestamp = DateTime.UtcNow,
                Exception = result.Exception?.Message
            };
        }

        private string DetermineOverallStatus(HealthStatus api, HealthStatus database)
        {
            if (api == HealthStatus.Unhealthy || database == HealthStatus.Unhealthy)
                return "Unhealthy";

            if (database == HealthStatus.Degraded)
                return "Degraded";

            return "Healthy";
        }

        private string GetUptime()
        {
            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }
    }
}
