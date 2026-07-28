using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Sqlite;

public sealed class SqliteHealthCheck : DbContext, IHealthCheck
{
    public SqliteHealthCheck(DbContextOptions<SqliteHealthCheck> options)
        : base(options) { }

    public SqliteHealthCheck(string? connectionString)
        : base(new DbContextOptionsBuilder<SqliteHealthCheck>().UseSqlite(connectionString 
            ?? throw new InvalidOperationException("Sqlite health check is missing its connection string.")).Options) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("Sqlite connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}