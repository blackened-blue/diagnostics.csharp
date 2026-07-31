using ArangoDBNetStandard.DatabaseApi;
using ArangoDBNetStandard.Transport.Http;

namespace Blackened.Blue.Diagnostics.HealthChecks.ArangoDb;

public static class OptionsBuilder
{
    public static IDatabaseApiClient UseArangoDb(string? connectionString)
    {
        if (connectionString is null)
            throw new InvalidOperationException("ArangoDb health check is missing its connection string.");

        string? host = null;
        var port = 8529;
        string? database = null;
        string? username = null;
        string? password = null;

        foreach (var entry in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pair = entry.Split('=', 2);
            if (pair.Length != 2)
                throw new ArgumentException($"Malformed ArangoDb connection string entry '{entry}'.", nameof(connectionString));

            switch (pair[0])
            {
                case "Host": host = pair[1]; break;
                case "Port": port = int.Parse(pair[1]); break;
                case "Database": database = pair[1]; break;
                case "Username": username = pair[1]; break;
                case "Password": password = pair[1]; break;
                default: throw new ArgumentException($"Unknown ArangoDb connection string property '{pair[0]}'.", nameof(connectionString));
            }
        }

        if (host is null || database is null || username is null || password is null)
            throw new InvalidOperationException("ArangoDb health check connection string is missing Host, Database, Username, or Password.");

        var transport = HttpApiTransport.UsingBasicAuth(new Uri($"http://{host}:{port}"), database, username, password);

        return new DatabaseApiClient(transport);
    }
}
