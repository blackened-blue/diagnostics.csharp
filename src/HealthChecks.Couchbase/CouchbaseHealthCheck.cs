using Couchbase;
using Couchbase.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Couchbase;

public sealed class CouchbaseHealthCheck : IHealthCheck
{
    private readonly ICluster _client;

    public CouchbaseHealthCheck(ICluster client)
        => _client = client;

    public CouchbaseHealthCheck(string? connectionString)
        => _client = OptionsBuilder.UseCouchbase(connectionString);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _client.PingAsync();

            if (report.Services.Values.SelectMany(endpoints => endpoints).Any(endpoint => endpoint.State != ServiceState.Ok))
                return HealthCheckResult.Unhealthy("Couchbase connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
