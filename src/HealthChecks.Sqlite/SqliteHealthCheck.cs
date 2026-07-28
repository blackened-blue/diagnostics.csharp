using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Sqlite;

public sealed class SqliteHealthCheck<T>(T target) : IHealthCheck
    where T : DbContext
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await target.Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("Sqlite connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}