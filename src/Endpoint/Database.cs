using Microsoft.EntityFrameworkCore;

namespace Endpoint;

public sealed class MySqlDbContext(DbContextOptions<MySqlDbContext> options) : Database(options) { }