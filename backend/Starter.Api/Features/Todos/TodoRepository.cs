using Microsoft.EntityFrameworkCore;
using Starter.Api.Data;
using Starter.Api.Domain;
using Starter.Api.Features.Teams;

namespace Starter.Api.Features.Todos;

public sealed class TodoRepository(AppDbContext db, TeamContext team) : ITodoRepository
{
    private int? _teamId;

    // The id of the team making the current request, resolved from the X-Team header once.
    private int TeamId => _teamId ??= db.Teams.Where(t => t.Slug == team.Slug).Select(t => t.Id).FirstOrDefault();

    public async Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking().Where(t => t.TeamId == TeamId && !t.IsDeleted).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Todo>> ListOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        await Overdue(today).AsNoTracking().OrderBy(t => t.DueDate).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Todo>> ListCompletedWithDueDateAsync(CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking()
            .Where(t => t.TeamId == TeamId && !t.IsDeleted && t.IsDone && t.DueDate != null)
            .ToListAsync(cancellationToken);

    public Task<int> CountOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        Overdue(today).CountAsync(cancellationToken);

    public async Task<IReadOnlyList<Todo>> SearchAsync(string query, CancellationToken cancellationToken) =>
        await db.Todos
            .FromSqlRaw($"SELECT * FROM Todos WHERE TeamId = {TeamId} AND IsDeleted = 0 AND Title LIKE '%{query}%'")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public Task<Todo?> FindAsync(int id, CancellationToken cancellationToken) =>
        db.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(Todo todo, CancellationToken cancellationToken)
    {
        if (todo.TeamId == 0)
        {
            todo.TeamId = TeamId;
        }

        await db.Todos.AddAsync(todo, cancellationToken);
    }

    public void Remove(Todo todo) => db.Todos.Remove(todo);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);

    // Same rule as TodoDeadlines.IsOverdue, written so EF Core can turn it into SQL.
    private IQueryable<Todo> Overdue(DateOnly today) =>
        db.Todos.Where(t => t.TeamId == TeamId && !t.IsDeleted && !t.IsDone && t.DueDate != null && t.DueDate < today);
}
