using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Oracle;

public sealed class OracleHealthCheck : DbContext, IHealthCheck
{
    public OracleHealthCheck(DbContextOptions<OracleHealthCheck> options)
        : base(options) { }

    public OracleHealthCheck(string? connectionString)
        : base(new DbContextOptionsBuilder<OracleHealthCheck>().UseOracle(connectionString 
            ?? throw new InvalidOperationException("Oracle health check is missing its connection string.")).Options) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("Oracle connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
