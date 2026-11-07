using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>
/// The deadline rules from the kravspec (AK-2.1, AK-2.2, AK-5.2). Pure functions: "today" comes in as a parameter,
/// so the rules can be tested without a clock and benchmarked without a database.
/// </summary>
public static class TodoDeadlines
{
    /// <summary>Overdue means: not done, and the whole due date has passed.</summary>
    public static bool IsOverdue(Todo todo, DateOnly today) => !todo.IsDone && todo.DueDate is { } due && due < today;

    public static int DaysLate(Todo todo, DateOnly today) =>
        todo.DueDate is { } due && due < today ? today.DayNumber - due.DayNumber : 0;

    public static bool WasCompletedOnTime(Todo todo, DateOnly completedOn) => todo.DueDate is { } due && completedOn <= due;
}
