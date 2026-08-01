using Couchbase;
using Couchbase.Analytics;
using Couchbase.Client.Transactions;
using Couchbase.Core.IO.Authentication.Authenticators;
using Couchbase.Diagnostics;
using Couchbase.Management.Analytics;
using Couchbase.Management.Buckets;
using Couchbase.Management.Eventing;
using Couchbase.Management.Query;
using Couchbase.Management.Search;
using Couchbase.Management.Users;
using Couchbase.Query;
using Couchbase.Search;

namespace Blackened.Blue.Diagnostics.HealthChecks.Couchbase;

public sealed class CouchbaseConnection : ICluster
{
    private readonly ICluster _cluster;

    public CouchbaseConnection(string connectionString)
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
        
        _cluster = global::Couchbase.Cluster.ConnectAsync(uriBuilder.Uri.GetLeftPart(UriPartial.Authority), username, password)
            .GetAwaiter().GetResult();
    }

    public void Dispose() => _cluster.Dispose();
    public ValueTask DisposeAsync() => _cluster.DisposeAsync();
    public void Authenticator(IAuthenticator authenticator) => _cluster.Authenticator(authenticator);
    public ValueTask<IBucket> BucketAsync(string name) => _cluster.BucketAsync(name);
    public Task<IPingReport> PingAsync(PingOptions? options = null) => _cluster.PingAsync(options);
    public Task WaitUntilReadyAsync(TimeSpan timeout, WaitUntilReadyOptions? options = null) => _cluster.WaitUntilReadyAsync(timeout, options);
    public Task<IDiagnosticsReport> DiagnosticsAsync(DiagnosticsOptions? options = null) => _cluster.DiagnosticsAsync(options);
    public Task<IQueryResult<T>> QueryAsync<T>(string statement, QueryOptions? options = null) => _cluster.QueryAsync<T>(statement, options);
    public Task<IAnalyticsResult<T>> AnalyticsQueryAsync<T>(string statement, AnalyticsOptions? options = null) => _cluster.AnalyticsQueryAsync<T>(statement, options);
    public Task<ISearchResult> SearchQueryAsync(string indexName, ISearchQuery query, SearchOptions? options = null) => _cluster.SearchQueryAsync(indexName, query, options);
    public IServiceProvider ClusterServices => _cluster.ClusterServices;
    public IQueryIndexManager QueryIndexes => _cluster.QueryIndexes;
    public IAnalyticsIndexManager AnalyticsIndexes => _cluster.AnalyticsIndexes;
    public ISearchIndexManager SearchIndexes => _cluster.SearchIndexes;
    public IBucketManager Buckets => _cluster.Buckets;
    public IUserManager Users => _cluster.Users;
    public IEventingFunctionManager EventingFunctions => _cluster.EventingFunctions;
    public Transactions Transactions => _cluster.Transactions;
}