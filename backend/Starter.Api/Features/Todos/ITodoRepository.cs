using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>Data access for todos. The service depends on this interface, which makes it testable without a database.</summary>
public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Open todos whose due date is before <paramref name="today"/>. Filtered in the database, not in memory.</summary>
    Task<IReadOnlyList<Todo>> ListOverdueAsync(DateOnly today, CancellationToken cancellationToken);

    /// <summary>Todos that have both a due date and are done: the raw material for the on-time metric.</summary>
    Task<IReadOnlyList<Todo>> ListCompletedWithDueDateAsync(CancellationToken cancellationToken);

    Task<int> CountOverdueAsync(DateOnly today, CancellationToken cancellationToken);

    /// <summary>Free-text search over titles.</summary>
    Task<IReadOnlyList<Todo>> SearchAsync(string query, CancellationToken cancellationToken);

    Task<Todo?> FindAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Todo todo, CancellationToken cancellationToken);

    void Remove(Todo todo);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
