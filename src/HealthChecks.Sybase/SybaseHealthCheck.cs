using AdoNetCore.AseClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Sybase;

public sealed class SybaseHealthCheck : IHealthCheck
{
    private readonly AseConnection _client;

    public SybaseHealthCheck(AseConnection client)
        => _client = client;

    public SybaseHealthCheck(string? connectionString)
        => _client = new AseConnection(connectionString
            ?? throw new InvalidOperationException("Sybase health check is missing its connection string."));

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
