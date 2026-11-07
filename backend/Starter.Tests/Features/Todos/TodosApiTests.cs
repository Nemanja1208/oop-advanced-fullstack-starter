using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>
/// Starts the real API in memory, with its own throwaway SQLite file and a fake clock, and talks to it over HTTP.
/// These tests check the whole chain: JSON in, controller, service, EF Core, database, JSON out.
/// </summary>
public sealed class TodosApiTests : IClassFixture<TodosApiTests.ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    private readonly HttpClient _client;
    private readonly FakeTimeProvider _clock;

    public TodosApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
        _clock = factory.Clock;
    }

    private DateOnly Today => DateOnly.FromDateTime(_clock.GetUtcNow().UtcDateTime);

    [Fact]
    public async Task Post_then_get_returns_the_created_todo()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new { title = "Write a test" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var todos = await _client.GetFromJsonAsync<List<TodoResponse>>("/api/todos", Json);

        Assert.Contains(todos!, t => t.Title == "Write a test");
    }

    [Fact]
    public async Task Post_with_empty_title_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Krav", "AK-1.2")]
    public async Task Post_with_a_due_date_in_the_past_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "Sent", dueDate = Today.AddDays(-1) });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Krav", "AK-4.2")]
    public async Task Post_with_an_unknown_priority_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "Konstig", priority = "Urgent" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Krav", "AK-2.3")]
    public async Task Overdue_endpoint_returns_a_todo_once_its_due_date_has_passed()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new { title = "Ring kunden", dueDate = Today });
        var todo = await created.Content.ReadFromJsonAsync<TodoResponse>(Json);
        _clock.Advance(TimeSpan.FromDays(3));

        var overdue = await _client.GetFromJsonAsync<List<OverdueTodoResponse>>("/api/todos/overdue", Json);

        Assert.Contains(overdue!, o => o.Id == todo!.Id && o.DaysLate == 3);
    }

    [Fact]
    [Trait("Krav", "AK-5.2")]
    public async Task Stats_endpoint_returns_the_numbers()
    {
        var response = await _client.GetAsync("/api/todos/stats");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<DeadlineStatsResponse>(Json));
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"starter-test-{Guid.NewGuid()}.db");

        // Noon UTC, so the Swedish date and the UTC date are the same day in these tests.
        public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 11, 16, 12, 0, 0, TimeSpan.Zero));

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("ConnectionStrings:Default", $"Data Source={_databasePath}");
            builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(Clock));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(_databasePath);
        }
    }
}
