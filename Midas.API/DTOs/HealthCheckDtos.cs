namespace Midas.API.DTOs
{
    /// <summary>
    /// DTO para resposta detalhada de um health check individual
    /// </summary>
  public class HealthCheckDetailResponse
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public double Duration { get; set; }
  public DateTime Timestamp { get; set; }
        public string Exception { get; set; }
    }

    /// <summary>
    /// DTO para resposta completa de todos os health checks
    /// </summary>
    public class CompleteHealthCheckResponse
    {
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Environment { get; set; }
        public HealthCheckDetailResponse ApiHealth { get; set; }
    public HealthCheckDetailResponse DatabaseHealth { get; set; }
        public string Uptime { get; set; }
    }

    /// <summary>
    /// DTO para resposta de readiness probe
    /// </summary>
    public class ReadinessCheckResponse
    {
        public bool IsReady { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Details { get; set; }
    }
}
