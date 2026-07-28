using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.MySql;

public sealed class MySqlHealthCheck : DbContext, IHealthCheck
{
    public MySqlHealthCheck(DbContextOptions<MySqlHealthCheck> options)
        : base(options) { }

    public MySqlHealthCheck(string? connectionString)
        : base(new DbContextOptionsBuilder<MySqlHealthCheck>().UseMySQL(connectionString 
            ?? throw new InvalidOperationException("MySql health check is missing its connection string.")).Options) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("MySql connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}