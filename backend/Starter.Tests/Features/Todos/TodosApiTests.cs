using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>Starts the real API in memory, with its own throwaway SQLite file, and talks to it over HTTP.</summary>
public sealed class TodosApiTests : IClassFixture<TodosApiTests.ApiFactory>
{
    private readonly HttpClient _client;

    public TodosApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_then_get_returns_the_created_todo()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Write a test"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var todos = await _client.GetFromJsonAsync<List<TodoResponse>>("/api/todos");

        Assert.Contains(todos!, t => t.Title == "Write a test");
    }

    [Fact]
    public async Task Post_with_empty_title_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"starter-test-{Guid.NewGuid()}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.UseSetting("ConnectionStrings:Default", $"Data Source={_databasePath}");

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(_databasePath);
        }
    }
}
