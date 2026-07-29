using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.CockroachDb;

public sealed class CockroachDbHealthCheck : DbContext, IHealthCheck
{
    public CockroachDbHealthCheck(DbContextOptions<CockroachDbHealthCheck> options)
        : base(options) { }

    public CockroachDbHealthCheck(string? connectionString)
        : base(new DbContextOptionsBuilder<CockroachDbHealthCheck>().UseNpgsql(connectionString
            ?? throw new InvalidOperationException("CockroachDb health check is missing its connection string.")).Options) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("CockroachDb connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
