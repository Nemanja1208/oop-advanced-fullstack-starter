using Microsoft.EntityFrameworkCore;
using Starter.Api.Data;
using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

public sealed class TodoRepository(AppDbContext db) : ITodoRepository
{
    public async Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Todo>> ListOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        await Overdue(today).AsNoTracking().OrderBy(t => t.DueDate).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Todo>> ListCompletedWithDueDateAsync(CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking().Where(t => t.IsDone && t.DueDate != null).ToListAsync(cancellationToken);

    public Task<int> CountOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        Overdue(today).CountAsync(cancellationToken);

    public Task<Todo?> FindAsync(int id, CancellationToken cancellationToken) =>
        db.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(Todo todo, CancellationToken cancellationToken) =>
        await db.Todos.AddAsync(todo, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);

    // Same rule as TodoDeadlines.IsOverdue, written so EF Core can turn it into SQL.
    private IQueryable<Todo> Overdue(DateOnly today) => db.Todos.Where(t => !t.IsDone && t.DueDate != null && t.DueDate < today);
}
