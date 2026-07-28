using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Blackened.Blue.Diagnostics.HealthChecks.MongoDb;

public sealed class MongoDbHealthCheck: IHealthCheck
{
    private readonly IMongoClient _connection;

    public MongoDbHealthCheck(IMongoClient connection)
        => _connection = connection;

    public MongoDbHealthCheck(string? connectionString)
        => _connection = new MongoClient(connectionString 
            ?? throw new InvalidOperationException("MongoDb health check is missing its connection string."));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _connection.GetDatabase("admin")
                .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}