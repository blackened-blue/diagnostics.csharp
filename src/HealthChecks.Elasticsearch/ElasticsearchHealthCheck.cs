using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Elasticsearch;

public sealed class ElasticsearchHealthCheck: IHealthCheck
{
    private readonly ElasticsearchClient _client;

    public ElasticsearchHealthCheck(ElasticsearchClient client)
        => _client = client;

    public ElasticsearchHealthCheck(string? connectionString)
        => _client = new ElasticsearchClient(new Uri(connectionString 
            ?? throw new InvalidOperationException("Elasticsearch health check is missing its connection string.")));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PingAsync(cancellationToken);

            if (!response.IsValidResponse)
                return HealthCheckResult.Unhealthy("Elasticsearch connection is unavailable.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}