using Microsoft.Extensions.Diagnostics.HealthChecks;
using Vertica.Data.VerticaClient;

namespace Blackened.Blue.Diagnostics.HealthChecks.Vertica;

public sealed class VerticaHealthCheck : IHealthCheck
{
    private readonly VerticaConnection _client;

    public VerticaHealthCheck(VerticaConnection client)
        => _client = client;

    public VerticaHealthCheck(string? connectionString)
        => _client = new VerticaConnection(connectionString
            ?? throw new InvalidOperationException("Vertica health check is missing its connection string."));

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
