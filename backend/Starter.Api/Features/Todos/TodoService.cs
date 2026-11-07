using Starter.Api.Domain;
using Starter.Api.Shared;

namespace Starter.Api.Features.Todos;

/// <summary>The rules for todos live here, not in the controller and not in the database.</summary>
public sealed class TodoService(ITodoRepository repository, DeadlineCalendar calendar)
{
    public async Task<OperationResult<IReadOnlyList<TodoResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var today = calendar.Today();
        var todos = await repository.ListAsync(cancellationToken);
        return OperationResult<IReadOnlyList<TodoResponse>>.Ok(
            TodoOrdering.ForWorkList(todos, today).Select(t => ToResponse(t, today)).ToList());
    }

    public async Task<OperationResult<IReadOnlyList<OverdueTodoResponse>>> ListOverdueAsync(CancellationToken cancellationToken)
    {
        var today = calendar.Today();
        var overdue = await repository.ListOverdueAsync(today, cancellationToken);
        return OperationResult<IReadOnlyList<OverdueTodoResponse>>.Ok(
            overdue.Select(t => new OverdueTodoResponse(t.Id, t.Title, t.DueDate!.Value, TodoDeadlines.DaysLate(t, today))).ToList());
    }

    public async Task<OperationResult<DeadlineStatsResponse>> StatsAsync(CancellationToken cancellationToken)
    {
        var completed = await repository.ListCompletedWithDueDateAsync(cancellationToken);
        var onTime = completed.Count(t => t.CompletedAt is { } at && TodoDeadlines.WasCompletedOnTime(t, calendar.DateOf(at)));
        var late = completed.Count - onTime;
        var overdueNow = await repository.CountOverdueAsync(calendar.Today(), cancellationToken);
        double? onTimeRate = completed.Count == 0 ? null : (double)onTime / completed.Count;

        return OperationResult<DeadlineStatsResponse>.Ok(new DeadlineStatsResponse(onTime, late, overdueNow, onTimeRate));
    }

    public async Task<OperationResult<TodoResponse>> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var title = request.Title?.Trim() ?? string.Empty;
        var today = calendar.Today();
        var errors = Validate(title, request, today);
        if (errors.Count > 0)
        {
            return OperationResult<TodoResponse>.Invalid([.. errors]);
        }

        var todo = new Todo
        {
            Title = title,
            CreatedAt = calendar.Now(),
            DueDate = request.DueDate,
            Priority = request.Priority ?? Priority.Normal,
        };
        await repository.AddAsync(todo, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return OperationResult<TodoResponse>.Ok(ToResponse(todo, today));
    }

    public async Task<OperationResult<TodoResponse>> MarkDoneAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await repository.FindAsync(id, cancellationToken);
        if (todo is null)
        {
            return OperationResult<TodoResponse>.NotFound($"Todo {id} was not found.");
        }

        todo.IsDone = true;
        todo.CompletedAt = calendar.Now();
        await repository.SaveChangesAsync(cancellationToken);
        return OperationResult<TodoResponse>.Ok(ToResponse(todo, calendar.Today()));
    }

    private static List<string> Validate(string title, CreateTodoRequest request, DateOnly today)
    {
        var errors = new List<string>();
        if (title.Length == 0)
        {
            errors.Add("Title is required.");
        }
        else if (title.Length > TodoRules.MaxTitleLength)
        {
            errors.Add($"Title can be at most {TodoRules.MaxTitleLength} characters.");
        }

        if (request.DueDate is { } due && due < today)
        {
            errors.Add($"Förfallodatumet {due:yyyy-MM-dd} har redan passerat.");
        }

        if (request.Priority is { } priority && !Enum.IsDefined(priority))
        {
            errors.Add("Priority must be Low, Normal or High.");
        }

        return errors;
    }

    private static TodoResponse ToResponse(Todo todo, DateOnly today) =>
        new(todo.Id, todo.Title, todo.IsDone, todo.CreatedAt, todo.DueDate, todo.Priority, TodoDeadlines.IsOverdue(todo, today), todo.CompletedAt);
}
