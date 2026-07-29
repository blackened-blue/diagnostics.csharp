using IBM.Data.Db2;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Db2;

public sealed class Db2HealthCheck : IHealthCheck
{
    private readonly DB2Connection _client;

    public Db2HealthCheck(DB2Connection client)
        => _client = client;

    public Db2HealthCheck(string? connectionString)
        => _client = new DB2Connection(connectionString
            ?? throw new InvalidOperationException("Db2 health check is missing its connection string."));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.OpenAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
