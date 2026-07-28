using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Oracle;

public sealed class OracleHealthCheck<T>(T target) : IHealthCheck
    where T : DbContext
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await target.Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("Oracle connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}