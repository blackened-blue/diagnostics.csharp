using ArangoDBNetStandard.DatabaseApi;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Blackened.Blue.Diagnostics.HealthChecks.ArangoDb;

public sealed class ArangoDbHealthCheck : IHealthCheck
{
    private readonly IDatabaseApiClient _client;

    public ArangoDbHealthCheck(IDatabaseApiClient client)
        => _client = client;

    public ArangoDbHealthCheck(string? connectionString)
        => _client = OptionsBuilder.UseArangoDb(connectionString);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.GetCurrentDatabaseInfoAsync();

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
