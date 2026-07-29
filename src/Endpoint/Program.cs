using Blackened.Blue.Diagnostics.HealthChecks.CockroachDb;
using Blackened.Blue.Diagnostics.HealthChecks.Couchbase;
using Blackened.Blue.Diagnostics.HealthChecks.Elasticsearch;
using Blackened.Blue.Diagnostics.HealthChecks.Kafka;
using Blackened.Blue.Diagnostics.HealthChecks.MongoDb;
using Blackened.Blue.Diagnostics.HealthChecks.MsSql;
using Blackened.Blue.Diagnostics.HealthChecks.MySql;
using Blackened.Blue.Diagnostics.HealthChecks.NpgSql;
using Blackened.Blue.Diagnostics.HealthChecks.Oracle;
using Blackened.Blue.Diagnostics.HealthChecks.Redis;
using Blackened.Blue.Diagnostics.HealthChecks.Sqlite;
using Confluent.Kafka;
using Couchbase;
using Elastic.Clients.Elasticsearch;
using Endpoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region CockroachDb

builder.Services.AddDbContext<CockroachDbDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CockroachDbConnection")
    ?? throw new InvalidOperationException("CockroachDb services could not be registered because no connection string has been configured for dependency injection.")));
builder.Services.AddDbContext<CockroachDbHealthCheck>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CockroachDbConnection")
    ?? throw new InvalidOperationException("CockroachDb services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<CockroachDbHealthCheck<CockroachDbDbContext>>(name: "CockroachDb", tags: ["ready"])
    .AddCheck<CockroachDbHealthCheck>(name: "CockroachDb (standalone)", tags: ["ready"])
    .AddCheck(name: "CockroachDb (connection string)", tags: ["ready"],
        instance: new CockroachDbHealthCheck(builder.Configuration.GetConnectionString("CockroachDbConnection")));

#endregion

#region Couchbase

builder.Services.AddSingleton<ICluster>(await Cluster.ConnectAsync(builder.Configuration.GetSection("Couchbase").Get<ClusterOptions>() 
    ?? throw new InvalidOperationException("Couchbase services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<CouchbaseHealthCheck>(name: "Couchbase", tags: ["ready"])
    .AddCheck(name: "Couchbase (connection string)", tags: ["ready"],
        instance: new CouchbaseHealthCheck(builder.Configuration.GetConnectionString("CouchbaseConnection")));

#endregion

#region Elasticsearch

builder.Services.AddSingleton(new ElasticsearchClient(new Uri(builder.Configuration.GetConnectionString("ElasticsearchConnection")
    ?? throw new InvalidOperationException("Elasticsearch services could not be registered because no connection string has been configured for dependency injection."))));

builder.Services.AddHealthChecks()
    .AddCheck<ElasticsearchHealthCheck>(name: "Elasticsearch", tags: ["ready"])
    .AddCheck(name: "Elasticsearch (connection string)", tags: ["ready"],
        instance: new ElasticsearchHealthCheck(builder.Configuration.GetConnectionString("ElasticsearchConnection")));

#endregion

#region Kafka

builder.Services.AddSingleton<IAdminClient>(new AdminClientBuilder(builder.Configuration.GetSection("Kafka:Producer").AsEnumerable(makePathsRelative: true)).Build());

builder.Services.AddHealthChecks()
    .AddCheck<KafkaHealthCheck>(name: "Kafka", tags: ["ready"])
    .AddCheck(name: "Kafka (connection string)", tags: ["ready"],
        instance: new KafkaHealthCheck(builder.Configuration.GetConnectionString("KafkaConnection")));

#endregion

#region MongoDb

builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration.GetConnectionString("MongoDbConnection")
    ?? throw new InvalidOperationException("MongoDb services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<MongoDbHealthCheck>(name: "MongoDb", tags: ["ready"])
    .AddCheck(name: "MongoDb (connection string)", tags: ["ready"],
        instance: new MongoDbHealthCheck(builder.Configuration.GetConnectionString("MongoDbConnection")));

#endregion

#region MsSql

builder.Services.AddDbContext<MsSqlDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection")
    ?? throw new InvalidOperationException("MsSql services could not be registered because no connection string has been configured for dependency injection.")));
builder.Services.AddDbContext<MsSqlHealthCheck>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection")
    ?? throw new InvalidOperationException("MsSql services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<MsSqlHealthCheck<MsSqlDbContext>>(name: "MsSql", tags: ["ready"])
    .AddCheck<MsSqlHealthCheck>(name: "MsSql (standalone)", tags: ["ready"])
    .AddCheck(name: "MsSql (connection string)", tags: ["ready"],
        instance: new MsSqlHealthCheck(builder.Configuration.GetConnectionString("MsSqlConnection")));

#endregion

#region MySql

builder.Services.AddDbContext<MySqlDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnection")
                                                                          ?? throw new InvalidOperationException("MySql services could not be registered because no connection string has been configured for dependency injection.")));
builder.Services.AddDbContext<MySqlHealthCheck>(options => options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnection")
                                                                            ?? throw new InvalidOperationException("MySql services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<MySqlHealthCheck<MySqlDbContext>>(name: "MySql", tags: ["ready"])
    .AddCheck<MySqlHealthCheck>(name: "MySql (standalone)", tags: ["ready"])
    .AddCheck(name: "MySql (connection string)", tags: ["ready"],
        instance: new MySqlHealthCheck(builder.Configuration.GetConnectionString("MySqlConnection")));

#endregion

#region NpgSql

builder.Services.AddDbContext<NpgSqlDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NpgSqlConnection")
    ?? throw new InvalidOperationException("NpgSql services could not be registered because no connection string has been configured for dependency injection.")));
builder.Services.AddDbContext<NpgSqlHealthCheck>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NpgSqlConnection")
    ?? throw new InvalidOperationException("NpgSql services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<NpgSqlHealthCheck<NpgSqlDbContext>>(name: "NpgSql", tags: ["ready"])
    .AddCheck<NpgSqlHealthCheck>(name: "NpgSql (standalone)", tags: ["ready"])
    .AddCheck(name: "NpgSql (connection string)", tags: ["ready"],
        instance: new NpgSqlHealthCheck(builder.Configuration.GetConnectionString("NpgSqlConnection")));

#endregion

#region Oracle

builder.Services.AddDbContext<OracleDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")
    ?? throw new InvalidOperationException("Oracle services could not be registered because no connection string has been configured for dependency injection.")));
builder.Services.AddDbContext<OracleHealthCheck>(options => options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")
    ?? throw new InvalidOperationException("Oracle services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<OracleHealthCheck<OracleDbContext>>(name: "Oracle", tags: ["ready"])
    .AddCheck<OracleHealthCheck>(name: "Oracle (standalone)", tags: ["ready"])
    .AddCheck(name: "Oracle (connection string)", tags: ["ready"],
        instance: new OracleHealthCheck(builder.Configuration.GetConnectionString("OracleConnection")));

#endregion

#region Redis

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")
    ?? throw new InvalidOperationException("Redis services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<RedisHealthCheck>(name: "Redis", tags: ["ready"])
    .AddCheck(name: "Redis (connection string)", tags: ["ready"],
        instance: new RedisHealthCheck(builder.Configuration.GetConnectionString("RedisConnection")));

#endregion

#region Sqlite

builder.Services.AddDbContext<SqliteDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")
    ?? throw new InvalidOperationException("Sqlite services could not be registered because no connection string has been configured for dependency injection.")));
builder.Services.AddDbContext<SqliteHealthCheck>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")
    ?? throw new InvalidOperationException("Sqlite services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<SqliteHealthCheck<SqliteDbContext>>(name: "Sqlite", tags: ["ready"])
    .AddCheck<SqliteHealthCheck>(name: "Sqlite (standalone)", tags: ["ready"])
    .AddCheck(name: "Sqlite (connection string)", tags: ["ready"],
        instance: new SqliteHealthCheck(builder.Configuration.GetConnectionString("SqliteConnection")));

#endregion

builder.Services.AddHealthChecks()
    .AddCheck(name: "self", check: () => HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapHealthChecks("/health/live", HealthCheck.Live);
app.MapHealthChecks("/health/ready", HealthCheck.Ready);
app.Run();