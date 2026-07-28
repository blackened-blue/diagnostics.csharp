using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.NpgSql;

public sealed class NpgSqlHealthCheck : DbContext, IHealthCheck
{
    public NpgSqlHealthCheck(DbContextOptions<NpgSqlHealthCheck> options)
        : base(options) { }

    public NpgSqlHealthCheck(string? connectionString)
        : base(new DbContextOptionsBuilder<NpgSqlHealthCheck>().UseNpgsql(connectionString 
            ?? throw new InvalidOperationException("NpgSql health check is missing its connection string.")).Options) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("NpgSql connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}