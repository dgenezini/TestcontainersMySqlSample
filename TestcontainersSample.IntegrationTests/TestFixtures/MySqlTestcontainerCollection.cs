namespace TestcontainersSample.IntegrationTests.TestFixtures;

[CollectionDefinition("MySqlTestcontainer Collection")]
public class MySqlTestcontainerCollection: ICollectionFixture<MySqlTestcontainerFixture>
{
}