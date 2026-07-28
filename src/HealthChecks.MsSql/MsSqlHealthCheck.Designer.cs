using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.MsSql;

public sealed class MsSqlHealthCheck : DbContext, IHealthCheck
{
    public MsSqlHealthCheck(DbContextOptions<MsSqlHealthCheck> options)
        : base(options) { }

    public MsSqlHealthCheck(string? connectionString)
        : base(new DbContextOptionsBuilder<MsSqlHealthCheck>().UseSqlServer(connectionString 
            ?? throw new InvalidOperationException("MsSql health check is missing its connection string.")).Options) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("MsSql connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}