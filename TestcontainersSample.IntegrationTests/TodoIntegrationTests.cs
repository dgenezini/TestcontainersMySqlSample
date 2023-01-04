using AutoFixture;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TestcontainersSample.DatabaseContext;
using TestcontainersSample.IntegrationTests.TestFixtures;

namespace TestcontainersSample.IntegrationTests;

[Collection("MySqlTestcontainer Collection")]
public class TodoIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly MySqlTestcontainer _mySqlTestcontainer;
    private readonly TodoContext _todoContext;

    public TodoIntegrationTests(WebApplicationFactory<Program> factory, 
        MySqlTestcontainerFixture mySqlTestcontainerFixture)
    {
        _factory = factory;
        _mySqlTestcontainer = mySqlTestcontainerFixture.MySqlTestcontainer;
        _todoContext = mySqlTestcontainerFixture.TodoContext;
    }

    [Fact]
    public async Task GetOneItem_Returns200WithItem()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var todoItem = fixture.Create<TodoItem>();

        _todoContext.TodoItems.Add(todoItem);

        await _todoContext.SaveChangesAsync();

        var HttpClient = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("MySqlConnectionString", _mySqlTestcontainer.ConnectionString);
            })
            .CreateClient();

        //Act
        var HttpResponse = await HttpClient.GetAsync($"/todo/{todoItem.ItemId}");

        //Assert
        HttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseJson = await HttpResponse.Content.ReadAsStringAsync();
        var todoItemResult = JsonSerializer.Deserialize<TodoItem>(responseJson);

        todoItemResult.Should().BeEquivalentTo(todoItem);
    }

    [Fact]
    public async Task GetOneInexistentItem_Returns404WithItem()
    {
        //Arrange
        var HttpClient = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("MySqlConnectionString", _mySqlTestcontainer.ConnectionString);
            })
            .CreateClient();

        //Act
        var HttpResponse = await HttpClient.GetAsync($"/todo/300");

        //Assert
        HttpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostOneItem_Returns201AndCreateItem()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var todoItem = fixture.Build<TodoItem>()
            .With(x => x.ItemId, 0)
            .Create();

        var HttpClient = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("MySqlConnectionString", _mySqlTestcontainer.ConnectionString);
            })
            .CreateClient();

        //Act
        var HttpResponse = await HttpClient.PostAsJsonAsync($"/todo", todoItem);

        //Assert
        HttpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseJson = await HttpResponse.Content.ReadAsStringAsync();
        var todoItemResult = JsonSerializer.Deserialize<TodoItem>(responseJson);

        HttpResponse.Headers.Location.Should().Be($"{HttpClient.BaseAddress}Todo/{todoItemResult!.ItemId}");

        var dbItem = await _todoContext.TodoItems
            .SingleAsync(a => a.ItemId == todoItemResult!.ItemId);

        dbItem.Description.Should().Be(todoItem.Description);
    }
}