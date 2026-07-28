using Blackened.Blue.Diagnostics.HealthChecks.MsSql;
using Blackened.Blue.Diagnostics.HealthChecks.MySql;
using Blackened.Blue.Diagnostics.HealthChecks.Oracle;
using Endpoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region My Sql

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

#region Ms Sql

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

builder.Services.AddHealthChecks()
    .AddCheck(name: "self", check: () => HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHealthChecks("/health/live", HealthCheck.Live);
app.MapHealthChecks("/health/ready", HealthCheck.Ready);
app.Run();