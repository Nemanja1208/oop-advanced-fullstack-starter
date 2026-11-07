using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>What a client sends to create a todo. Due date and priority are optional (AK-1.3, AK-4.1).</summary>
public sealed record CreateTodoRequest(string Title, DateOnly? DueDate = null, Priority? Priority = null);

/// <summary>What a client gets back. This is the shape the frontend depends on.</summary>
public sealed record TodoResponse(
    int Id,
    string Title,
    bool IsDone,
    DateTimeOffset CreatedAt,
    DateOnly? DueDate,
    Priority Priority,
    bool IsOverdue,
    DateTimeOffset? CompletedAt);

/// <summary>One row in the overdue list (AK-2.3).</summary>
public sealed record OverdueTodoResponse(int Id, string Title, DateOnly DueDate, int DaysLate);

/// <summary>
/// The customer's success metric (AK-5.2). <c>OnTimeRate</c> is null until something with a due date has been
/// completed, because "0 %" and "no data yet" are different answers.
/// </summary>
public sealed record DeadlineStatsResponse(int CompletedOnTime, int CompletedLate, int OverdueNow, double? OnTimeRate);
