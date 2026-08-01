using Couchbase;
using Couchbase.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Couchbase;

public sealed class CouchbaseHealthCheck : IHealthCheck
{
    private readonly ICluster _cluster;

    public CouchbaseHealthCheck(ICluster client)
        => _cluster = client;

    public CouchbaseHealthCheck(string? connectionString)
        => _cluster = new CouchbaseConnection(connectionString
            ?? throw new InvalidOperationException("Couchbase health check is missing its connection string."));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _cluster.PingAsync();

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
