using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using TestcontainersSample.DatabaseContext;

namespace TestcontainersSample.IntegrationTests.TestFixtures;

public class MySqlTestcontainerFixture : IAsyncLifetime
{
    public MySqlTestcontainer MySqlTestcontainer { get; private set; } = default!;
    public TodoContext TodoContext { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        MySqlTestcontainer = new TestcontainersBuilder<MySqlTestcontainer>()
            .WithDatabase(new MySqlTestcontainerConfiguration
            {
                Password = "Test1234",
                Database = "TestDB"
            })
            .WithImage("mysql:8.0.31-oracle")
            .Build();

        await MySqlTestcontainer.StartAsync();

        TodoContext = TodoContext
            .CreateFromConnectionString(MySqlTestcontainer.ConnectionString);

        // Creates the database if it does not exists
        await TodoContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await MySqlTestcontainer.DisposeAsync();

        await TodoContext.DisposeAsync();
    }
}