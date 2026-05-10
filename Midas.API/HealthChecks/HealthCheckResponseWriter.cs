using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Midas.API.HealthChecks
{
    public static class HealthCheckResponseWriter
    {
        public static async Task WriteJsonResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                uptime = GetUptime(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    responseTime = entry.Value.Duration.TotalMilliseconds,
                    exception = entry.Value.Exception?.Message
                }).ToList()
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            await context.Response.WriteAsync(json);
        }

        private static string GetUptime()
        {
            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
        }
    }
}
