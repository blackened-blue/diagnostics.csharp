using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Endpoint;

public static class HealthCheck
{
    public static HealthCheckOptions Live => new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = (context, report) => context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            version = "1.0.0"
        })
    };

    public static HealthCheckOptions Ready => new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = (context, report) => context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            version = "1.0.0",
            stack = report.Entries.Select(entry => new { name = entry.Key, status = entry.Value.Status.ToString() }).ToList()
        })
    };
}