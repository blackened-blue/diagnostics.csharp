using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Blackened.Blue.Diagnostics.HealthChecks.MongoDb;

public sealed class MongoDbHealthCheck: IHealthCheck
{
    private readonly IMongoClient _client;

    public MongoDbHealthCheck(IMongoClient client)
        => _client = client;

    public MongoDbHealthCheck(string? connectionString)
        => _client = new MongoClient(connectionString 
            ?? throw new InvalidOperationException("MongoDb health check is missing its connection string."));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.GetDatabase("admin")
                .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}