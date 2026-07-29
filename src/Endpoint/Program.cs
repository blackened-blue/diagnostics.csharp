using Blackened.Blue.Diagnostics.HealthChecks.ClickHouse;
using Blackened.Blue.Diagnostics.HealthChecks.CockroachDb;
using Blackened.Blue.Diagnostics.HealthChecks.Couchbase;
using Blackened.Blue.Diagnostics.HealthChecks.Db2;
using Blackened.Blue.Diagnostics.HealthChecks.Elasticsearch;
using Blackened.Blue.Diagnostics.HealthChecks.Exasol;
using Blackened.Blue.Diagnostics.HealthChecks.Greenplum;
using Blackened.Blue.Diagnostics.HealthChecks.Kafka;
using Blackened.Blue.Diagnostics.HealthChecks.MongoDb;
using Blackened.Blue.Diagnostics.HealthChecks.MsSql;
using Blackened.Blue.Diagnostics.HealthChecks.MySql;
using Blackened.Blue.Diagnostics.HealthChecks.NpgSql;
using Blackened.Blue.Diagnostics.HealthChecks.Oracle;
using Blackened.Blue.Diagnostics.HealthChecks.Redis;
using Blackened.Blue.Diagnostics.HealthChecks.Sqlite;
using Blackened.Blue.Diagnostics.HealthChecks.Sybase;
using Blackened.Blue.Diagnostics.HealthChecks.Vertica;
using AdoNetCore.AseClient;
using ClickHouse.Client.ADO;
using Confluent.Kafka;
using Couchbase;
using Elastic.Clients.Elasticsearch;
using Endpoint;
using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Data.Odbc;
using Vertica.Data.VerticaClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region ClickHouse

builder.Services.AddSingleton(new ClickHouseConnection(builder.Configuration.GetConnectionString("ClickHouseConnection")
    ?? throw new InvalidOperationException("ClickHouse services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<ClickHouseHealthCheck>(name: "ClickHouse", tags: ["ready"])
    .AddCheck(name: "ClickHouse (connection string)", tags: ["ready"],
        instance: new ClickHouseHealthCheck(builder.Configuration.GetConnectionString("ClickHouseConnection")));

#endregion

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

#region Db2

builder.Services.AddSingleton(new DB2Connection(builder.Configuration.GetConnectionString("Db2Connection")
    ?? throw new InvalidOperationException("Db2 services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<Db2HealthCheck>(name: "Db2", tags: ["ready"])
    .AddCheck(name: "Db2 (connection string)", tags: ["ready"],
        instance: new Db2HealthCheck(builder.Configuration.GetConnectionString("Db2Connection")));

#endregion

#region Elasticsearch

builder.Services.AddSingleton(new ElasticsearchClient(new Uri(builder.Configuration.GetConnectionString("ElasticsearchConnection")
    ?? throw new InvalidOperationException("Elasticsearch services could not be registered because no connection string has been configured for dependency injection."))));

builder.Services.AddHealthChecks()
    .AddCheck<ElasticsearchHealthCheck>(name: "Elasticsearch", tags: ["ready"])
    .AddCheck(name: "Elasticsearch (connection string)", tags: ["ready"],
        instance: new ElasticsearchHealthCheck(builder.Configuration.GetConnectionString("ElasticsearchConnection")));

#endregion

#region Exasol

builder.Services.AddSingleton(new OdbcConnection(builder.Configuration.GetConnectionString("ExasolConnection")
    ?? throw new InvalidOperationException("Exasol services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<ExasolHealthCheck>(name: "Exasol", tags: ["ready"])
    .AddCheck(name: "Exasol (connection string)", tags: ["ready"],
        instance: new ExasolHealthCheck(builder.Configuration.GetConnectionString("ExasolConnection")));

#endregion

#region Greenplum

builder.Services.AddDbContext<GreenplumDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("GreenplumConnection")
    ?? throw new InvalidOperationException("Greenplum services could not be registered because no connection string has been configured for dependency injection.")));
builder.Services.AddDbContext<GreenplumHealthCheck>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("GreenplumConnection")
    ?? throw new InvalidOperationException("Greenplum services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<GreenplumHealthCheck<GreenplumDbContext>>(name: "Greenplum", tags: ["ready"])
    .AddCheck<GreenplumHealthCheck>(name: "Greenplum (standalone)", tags: ["ready"])
    .AddCheck(name: "Greenplum (connection string)", tags: ["ready"],
        instance: new GreenplumHealthCheck(builder.Configuration.GetConnectionString("GreenplumConnection")));

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

#region Sybase

builder.Services.AddSingleton(new AseConnection(builder.Configuration.GetConnectionString("SybaseConnection")
    ?? throw new InvalidOperationException("Sybase services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<SybaseHealthCheck>(name: "Sybase", tags: ["ready"])
    .AddCheck(name: "Sybase (connection string)", tags: ["ready"],
        instance: new SybaseHealthCheck(builder.Configuration.GetConnectionString("SybaseConnection")));

#endregion

#region Vertica

builder.Services.AddSingleton(new VerticaConnection(builder.Configuration.GetConnectionString("VerticaConnection")
    ?? throw new InvalidOperationException("Vertica services could not be registered because no connection string has been configured for dependency injection.")));

builder.Services.AddHealthChecks()
    .AddCheck<VerticaHealthCheck>(name: "Vertica", tags: ["ready"])
    .AddCheck(name: "Vertica (connection string)", tags: ["ready"],
        instance: new VerticaHealthCheck(builder.Configuration.GetConnectionString("VerticaConnection")));

#endregion

builder.Services.AddHealthChecks()
    .AddCheck(name: "self", check: () => HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapHealthChecks("/health/live", HealthCheck.Live);
app.MapHealthChecks("/health/ready", HealthCheck.Ready);
app.Run();