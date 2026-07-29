using Couchbase;

namespace Blackened.Blue.Diagnostics.HealthChecks.Couchbase;

public static class OptionsBuilder
{
    public static ICluster UseCouchbase(string? connectionString)
    {
        if (connectionString is null)
            throw new InvalidOperationException("Couchbase health check is missing its connection string.");

        var uriBuilder = new UriBuilder(connectionString);
        var username = uriBuilder.UserName;
        var password = uriBuilder.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new InvalidOperationException("Couchbase health check connection string is missing its embedded credentials (couchbase://username:password@host).");

        uriBuilder.UserName = string.Empty;
        uriBuilder.Password = string.Empty;

        return Cluster.ConnectAsync(uriBuilder.Uri.GetLeftPart(UriPartial.Authority), username, password)
            .GetAwaiter().GetResult();
    }
}
