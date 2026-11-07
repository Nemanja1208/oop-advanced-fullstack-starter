using Microsoft.Extensions.Time.Testing;
using Starter.Api.Domain;
using Starter.Api.Features.Todos;
using Starter.Api.Shared;

namespace Starter.Tests.Features.Todos;

public sealed class TodoServiceTests
{
    private static readonly TimeZoneInfo Stockholm = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    // Monday 16 November 2026, 09:00 in Stockholm.
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 11, 16, 8, 0, 0, TimeSpan.Zero));
    private readonly FakeTodoRepository _repository = new();
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _service = new TodoService(_repository, new DeadlineCalendar(_clock, Stockholm));
    }

    private static DateOnly Today => new(2026, 11, 16);

    [Fact]
    public async Task Create_trims_title_and_saves_the_todo()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("  Buy milk  "), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Buy milk", result.Data!.Title);
        Assert.Single(_repository.Saved);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_returns_validation_error_when_title_is_empty(string title)
    {
        var result = await _service.CreateAsync(new CreateTodoRequest(title), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ErrorKind.Validation, result.Kind);
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    public async Task Create_returns_validation_error_when_title_is_too_long()
    {
        var tooLong = new string('x', TodoRules.MaxTitleLength + 1);

        var result = await _service.CreateAsync(new CreateTodoRequest(tooLong), CancellationToken.None);

        Assert.Equal(ErrorKind.Validation, result.Kind);
    }

    [Fact]
    [Trait("Krav", "AK-1.1")]
    public async Task Create_saves_a_due_date_of_today()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("Skicka offert", Today), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(Today, result.Data!.DueDate);
    }

    [Fact]
    [Trait("Krav", "AK-1.2")]
    public async Task Create_rejects_a_due_date_that_has_already_passed()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("För sent", Today.AddDays(-1)), CancellationToken.None);

        Assert.Equal(ErrorKind.Validation, result.Kind);
        Assert.Contains(result.Errors, e => e.Contains("passerat"));
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    [Trait("Krav", "AK-1.3")]
    public async Task Create_without_due_date_saves_a_todo_without_due_date()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("När du hinner"), CancellationToken.None);

        Assert.Null(result.Data!.DueDate);
    }

    [Fact]
    [Trait("Krav", "AK-4.1")]
    public async Task Create_without_priority_gives_normal_priority()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("Vanlig"), CancellationToken.None);

        Assert.Equal(Priority.Normal, result.Data!.Priority);
    }

    [Fact]
    [Trait("Krav", "AK-4.2")]
    public async Task Create_rejects_an_unknown_priority()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("Konstig", Priority: (Priority)7), CancellationToken.None);

        Assert.Equal(ErrorKind.Validation, result.Kind);
    }

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public async Task Regression_a_todo_due_today_is_not_overdue_in_the_morning()
    {
        // Saras buggrapport: "Klockan nio på morgonen står det att Skicka offert är en dag sen."
        await _service.CreateAsync(new CreateTodoRequest("Skicka offert", Today), CancellationToken.None);

        var overdue = await _service.ListOverdueAsync(CancellationToken.None);

        Assert.Empty(overdue.Data!);
    }

    [Fact]
    [Trait("Krav", "AK-2.3")]
    public async Task ListOverdue_returns_only_overdue_todos_with_days_late()
    {
        await _service.CreateAsync(new CreateTodoRequest("Idag", Today), CancellationToken.None);
        await _service.CreateAsync(new CreateTodoRequest("Igår", Today), CancellationToken.None);
        _clock.Advance(TimeSpan.FromDays(2));

        var overdue = await _service.ListOverdueAsync(CancellationToken.None);

        Assert.Equal(2, overdue.Data!.Count);
        Assert.All(overdue.Data, o => Assert.Equal(2, o.DaysLate));
    }

    [Fact]
    [Trait("Krav", "AK-3.1")]
    public async Task List_returns_the_work_list_order_and_flags_overdue_todos()
    {
        await _service.CreateAsync(new CreateTodoRequest("Utan datum"), CancellationToken.None);
        await _service.CreateAsync(new CreateTodoRequest("Snart", Today), CancellationToken.None);
        _clock.Advance(TimeSpan.FromDays(1));

        var list = await _service.ListAsync(CancellationToken.None);

        Assert.Equal("Snart", list.Data![0].Title);
        Assert.True(list.Data[0].IsOverdue);
        Assert.False(list.Data[1].IsOverdue);
    }

    [Fact]
    public async Task MarkDone_returns_not_found_for_unknown_id()
    {
        var result = await _service.MarkDoneAsync(42, CancellationToken.None);

        Assert.Equal(ErrorKind.NotFound, result.Kind);
    }

    [Fact]
    [Trait("Krav", "AK-5.1")]
    public async Task MarkDone_records_when_the_todo_was_completed()
    {
        var created = await _service.CreateAsync(new CreateTodoRequest("Gör klart"), CancellationToken.None);

        var done = await _service.MarkDoneAsync(created.Data!.Id, CancellationToken.None);

        Assert.Equal(_clock.GetUtcNow(), done.Data!.CompletedAt);
    }

    [Fact]
    [Trait("Krav", "AK-5.2")]
    public async Task Stats_counts_on_time_late_and_overdue_and_computes_the_on_time_rate()
    {
        var onTime = await _service.CreateAsync(new CreateTodoRequest("I tid", Today), CancellationToken.None);
        var late = await _service.CreateAsync(new CreateTodoRequest("Sent", Today), CancellationToken.None);
        await _service.CreateAsync(new CreateTodoRequest("Fortfarande öppen", Today), CancellationToken.None);
        await _service.MarkDoneAsync(onTime.Data!.Id, CancellationToken.None);
        _clock.Advance(TimeSpan.FromDays(1));
        await _service.MarkDoneAsync(late.Data!.Id, CancellationToken.None);

        var stats = (await _service.StatsAsync(CancellationToken.None)).Data!;

        Assert.Equal(1, stats.CompletedOnTime);
        Assert.Equal(1, stats.CompletedLate);
        Assert.Equal(1, stats.OverdueNow);
        Assert.Equal(0.5, stats.OnTimeRate);
    }

    [Fact]
    [Trait("Krav", "AK-5.2")]
    public async Task Stats_has_no_on_time_rate_before_anything_with_a_due_date_is_completed()
    {
        var stats = (await _service.StatsAsync(CancellationToken.None)).Data!;

        Assert.Null(stats.OnTimeRate);
    }
}
