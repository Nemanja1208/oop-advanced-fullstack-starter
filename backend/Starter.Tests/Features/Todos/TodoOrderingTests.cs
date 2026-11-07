using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

public sealed class TodoOrderingTests
{
    private static readonly DateOnly Today = new(2026, 11, 16);

    [Fact]
    [Trait("Krav", "AK-3.1")]
    public void Work_list_puts_overdue_first_then_nearest_date_then_highest_priority_and_undated_last()
    {
        var undated = new Todo { Id = 1, Title = "Utan datum" };
        var nextWeekHigh = new Todo { Id = 2, Title = "Nästa vecka, hög", DueDate = Today.AddDays(7), Priority = Priority.High };
        var todayLow = new Todo { Id = 3, Title = "Idag, låg", DueDate = Today, Priority = Priority.Low };
        var todayHigh = new Todo { Id = 4, Title = "Idag, hög", DueDate = Today, Priority = Priority.High };
        var overdue = new Todo { Id = 5, Title = "Försenad", DueDate = Today.AddDays(-2) };

        var ordered = TodoOrdering.ForWorkList([undated, nextWeekHigh, todayLow, todayHigh, overdue], Today);

        Assert.Equal(["Försenad", "Idag, hög", "Idag, låg", "Nästa vecka, hög", "Utan datum"], ordered.Select(t => t.Title));
    }

    [Fact]
    [Trait("Krav", "AK-2.2")]
    public void A_done_todo_with_an_old_date_is_not_treated_as_overdue_in_the_list()
    {
        var doneLongAgo = new Todo { Id = 1, Title = "Klar", DueDate = Today.AddDays(-5), IsDone = true };
        var overdue = new Todo { Id = 2, Title = "Försenad", DueDate = Today.AddDays(-1) };

        var ordered = TodoOrdering.ForWorkList([doneLongAgo, overdue], Today);

        Assert.Equal("Försenad", ordered[0].Title);
    }
}
