using Cassandra;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Cassandra;

public sealed class CassandraHealthCheck : IHealthCheck
{
    private readonly ICluster _client;

    public CassandraHealthCheck(ICluster client)
        => _client = client;

    public CassandraHealthCheck(string? connectionString)
        => _client = OptionsBuilder.UseCassandra(connectionString);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.ConnectAsync();

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
