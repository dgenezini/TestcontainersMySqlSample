using Microsoft.EntityFrameworkCore;

namespace TestcontainersMySqlSample.DatabaseContext;

public class TodoContext : DbContext
{
    public DbSet<TodoItem> TodoItems { get; set; }

    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    public static TodoContext CreateFromConnectionString(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
        optionsBuilder.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString));

        return new TodoContext(optionsBuilder.Options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(true);
        });

        base.OnModelCreating(modelBuilder);
    }
}
