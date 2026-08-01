using System.Net;
using Cassandra;

namespace Blackened.Blue.Diagnostics.HealthChecks.Cassandra;

public sealed class CassandraConnection : ICluster
{
    private readonly ICluster _cluster;

    public CassandraConnection(string? connectionString)
    {
        if (connectionString is null)
            throw new InvalidOperationException("Cassandra health check is missing its connection string.");

        string? host = null;
        int port = ProtocolOptions.DefaultPort;
        string? keyspace = null;
        string? username = null;
        string? password = null;

        foreach (var entry in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pair = entry.Split('=', 2);
            if (pair.Length != 2)
                throw new ArgumentException($"Malformed Cassandra connection string entry '{entry}'.", nameof(connectionString));

            switch (pair[0])
            {
                case "Host": host = pair[1]; break;
                case "Port": port = int.Parse(pair[1]); break;
                case "Keyspace": keyspace = pair[1]; break;
                case "Username": username = pair[1]; break;
                case "Password": password = pair[1]; break;
                default: throw new ArgumentException($"Unknown Cassandra connection string property '{pair[0]}'.", nameof(connectionString));
            }
        }

        if (host is null)
            throw new InvalidOperationException("Cassandra health check connection string is missing its Host.");

        var builder = global::Cassandra.Cluster.Builder()
            .AddContactPoint(host)
            .WithPort(port);

        if (username is not null && password is not null)
            builder = builder.WithCredentials(username, password);

        if (keyspace is not null)
            builder = builder.WithDefaultKeyspace(keyspace);

        _cluster = builder.Build();
    }

    public void Dispose() => _cluster.Dispose();
    public ICollection<Host> AllHosts() => _cluster.AllHosts();
    public ISession Connect() => _cluster.Connect();
    public ISession Connect(string keyspace) => _cluster.Connect(keyspace);
    public Task<ISession> ConnectAsync() => _cluster.ConnectAsync();
    public Task<ISession> ConnectAsync(string keyspace) => _cluster.ConnectAsync(keyspace);
    public Host GetHost(IPEndPoint address) => _cluster.GetHost(address);
    public ICollection<Host> GetReplicas(byte[] partitionKey) => _cluster.GetReplicas(partitionKey);
    public ICollection<Host> GetReplicas(string keyspace, byte[] partitionKey) => _cluster.GetReplicas(keyspace, partitionKey);
    public void Shutdown(int timeoutMs = -1) => _cluster.Shutdown(timeoutMs);
    public Task ShutdownAsync(int timeoutMs = -1) => _cluster.ShutdownAsync(timeoutMs);
    public Task<bool> RefreshSchemaAsync(string keyspace = null, string table = null) => _cluster.RefreshSchemaAsync(keyspace, table);
    public bool RefreshSchema(string keyspace = null, string table = null) => _cluster.RefreshSchema(keyspace, table);
    public Metadata Metadata => _cluster.Metadata;
    public Configuration Configuration => _cluster.Configuration;
    public event Action<Host>? HostAdded { add => _cluster.HostAdded += value; remove => _cluster.HostAdded -= value; }
    public event Action<Host>? HostRemoved { add => _cluster.HostRemoved += value; remove => _cluster.HostRemoved -= value; }
}
