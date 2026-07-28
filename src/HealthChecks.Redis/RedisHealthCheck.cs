using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Blackened.Blue.Diagnostics.HealthChecks.Redis;

public sealed class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _client;
    
    public RedisHealthCheck(IConnectionMultiplexer client)
        => _client = client;
    
    public RedisHealthCheck(string? connectionString)
        => _client = ConnectionMultiplexer.Connect(connectionString 
            ?? throw new InvalidOperationException("Redis health check is missing its connection string."));
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.GetDatabase().PingAsync();
            
            return HealthCheckResult.Healthy(); 
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}