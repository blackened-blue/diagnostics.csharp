using Cassandra;

namespace Blackened.Blue.Diagnostics.HealthChecks.Cassandra;

public static class OptionsBuilder
{
    public static ICluster UseCassandra(string? connectionString)
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

        return builder.Build();
    }
}
