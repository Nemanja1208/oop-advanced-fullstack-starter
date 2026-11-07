using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>The order of the work list (AK-3.1): the most urgent thing is always at the top.</summary>
public static class TodoOrdering
{
    public static IReadOnlyList<Todo> ForWorkList(IEnumerable<Todo> todos, DateOnly today) =>
        todos
            .OrderBy(t => t.IsDone)
            .ThenByDescending(t => TodoDeadlines.IsOverdue(t, today))
            .ThenBy(t => t.DueDate is null)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ThenByDescending(t => t.Id)
            .ToList();
}
