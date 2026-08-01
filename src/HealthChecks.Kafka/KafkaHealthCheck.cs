using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Kafka;

public sealed class KafkaHealthCheck: IHealthCheck
{
    private readonly IAdminClient _client;

    [ActivatorUtilitiesConstructor]
    public KafkaHealthCheck(IAdminClient client)
        => _client = client;

    public KafkaHealthCheck(IEnumerable<KeyValuePair<string, string>> config)
        => _client = new AdminClientBuilder(config).Build();

    public KafkaHealthCheck(string? connectionString)
        : this(new KafkaConnection(connectionString
            ?? throw new InvalidOperationException("Kafka health check is missing its connection string."))) { }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _client.GetMetadata(TimeSpan.FromSeconds(5));

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}