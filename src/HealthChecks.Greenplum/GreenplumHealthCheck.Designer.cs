using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Greenplum;

public sealed class GreenplumHealthCheck : DbContext, IHealthCheck
{
    public GreenplumHealthCheck(DbContextOptions<GreenplumHealthCheck> options)
        : base(options) { }

    public GreenplumHealthCheck(string? connectionString)
        : base(new DbContextOptionsBuilder<GreenplumHealthCheck>().UseNpgsql(connectionString
            ?? throw new InvalidOperationException("Greenplum health check is missing its connection string.")).Options) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("Greenplum connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
