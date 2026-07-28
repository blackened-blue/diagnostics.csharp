using Microsoft.EntityFrameworkCore;

namespace Endpoint;

public sealed class MySqlDbContext(DbContextOptions<MySqlDbContext> options) : Database(options) { }
public sealed class MsSqlDbContext(DbContextOptions<MsSqlDbContext> options) : Database(options) { }
public sealed class OracleDbContext(DbContextOptions<OracleDbContext> options) : Database(options) { }