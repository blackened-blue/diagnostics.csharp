using System.Data.Odbc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.Exasol;

public sealed class ExasolHealthCheck : IHealthCheck
{
    private readonly OdbcConnection _client;

    public ExasolHealthCheck(OdbcConnection client)
        => _client = client;

    public ExasolHealthCheck(string? connectionString)
        => _client = new OdbcConnection(connectionString
            ?? throw new InvalidOperationException("Exasol health check is missing its connection string."));

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