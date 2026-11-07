using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>Pure rules: no clock, no database. "Today" is passed in, so every case is one line of setup.</summary>
public sealed class TodoDeadlinesTests
{
    private static readonly DateOnly Today = new(2026, 11, 16);

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void A_todo_due_today_is_not_overdue()
    {
        var todo = new Todo { DueDate = Today };

        Assert.False(TodoDeadlines.IsOverdue(todo, Today));
    }

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void A_todo_due_yesterday_is_overdue()
    {
        var todo = new Todo { DueDate = Today.AddDays(-1) };

        Assert.True(TodoDeadlines.IsOverdue(todo, Today));
    }

    [Fact]
    [Trait("Krav", "AK-2.2")]
    public void A_done_todo_is_never_overdue()
    {
        var todo = new Todo { DueDate = Today.AddDays(-10), IsDone = true };

        Assert.False(TodoDeadlines.IsOverdue(todo, Today));
    }

    [Fact]
    [Trait("Krav", "AK-1.3")]
    public void A_todo_without_due_date_is_never_overdue()
    {
        Assert.False(TodoDeadlines.IsOverdue(new Todo(), Today));
    }

    [Fact]
    [Trait("Krav", "AK-2.3")]
    public void DaysLate_counts_whole_days_after_the_due_date()
    {
        var todo = new Todo { DueDate = Today.AddDays(-3) };

        Assert.Equal(3, TodoDeadlines.DaysLate(todo, Today));
    }

    [Theory]
    [Trait("Krav", "AK-5.2")]
    [InlineData(0, true)]
    [InlineData(-1, true)]
    [InlineData(1, false)]
    public void WasCompletedOnTime_compares_completion_date_with_due_date(int daysAfterDue, bool expected)
    {
        var todo = new Todo { DueDate = Today, IsDone = true };

        Assert.Equal(expected, TodoDeadlines.WasCompletedOnTime(todo, Today.AddDays(daysAfterDue)));
    }
}
