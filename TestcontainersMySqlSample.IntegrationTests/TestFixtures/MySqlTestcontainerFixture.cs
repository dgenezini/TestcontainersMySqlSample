using Testcontainers.MySql;
using TestcontainersMySqlSample.DatabaseContext;

namespace TestcontainersMySqlSample.IntegrationTests.TestFixtures;

public class MySqlTestcontainerFixture : IAsyncLifetime
{
    public MySqlContainer MySqlContainer { get; private set; } = default!;
    public TodoContext TodoContext { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        MySqlContainer = new MySqlBuilder()
            .WithDatabase("TestDB")
            .WithPassword("Test1234")
            .WithImage("mysql:8.0.31-oracle")
            .Build();

        await MySqlContainer.StartAsync();

        TodoContext = TodoContext
            .CreateFromConnectionString(MySqlContainer.GetConnectionString());

        // Creates the database if it does not exists
        await TodoContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await MySqlContainer.DisposeAsync();

        await TodoContext.DisposeAsync();
    }
}