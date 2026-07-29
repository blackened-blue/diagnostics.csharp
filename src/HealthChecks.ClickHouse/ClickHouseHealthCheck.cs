using ClickHouse.Client.ADO;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.ClickHouse;

public sealed class ClickHouseHealthCheck : IHealthCheck
{
    private readonly ClickHouseConnection _client;

    public ClickHouseHealthCheck(ClickHouseConnection client)
        => _client = client;

    public ClickHouseHealthCheck(string? connectionString)
        => _client = new ClickHouseConnection(connectionString
            ?? throw new InvalidOperationException("ClickHouse health check is missing its connection string."));

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