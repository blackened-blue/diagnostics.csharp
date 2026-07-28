using Microsoft.EntityFrameworkCore;

namespace Endpoint;

public class HelloWord
{
    public string Message {get; set;}
}

public abstract class Database(DbContextOptions options) : DbContext(options)
{
    public required DbSet<HelloWord> HelloWord  { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HelloWord>().HasNoKey();
    }
}