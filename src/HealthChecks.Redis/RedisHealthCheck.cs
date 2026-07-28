using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Blackened.Blue.Diagnostics.HealthChecks.Redis;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _connection;
    
    public RedisHealthCheck(IConnectionMultiplexer connection)
        => _connection = connection;
    
    public RedisHealthCheck(string? connectionString)
        => _connection = ConnectionMultiplexer.Connect(connectionString 
            ?? throw new InvalidOperationException("Redis health check is missing its connection string."));
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _connection.GetDatabase().PingAsync();
            
            return HealthCheckResult.Healthy(); 
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}