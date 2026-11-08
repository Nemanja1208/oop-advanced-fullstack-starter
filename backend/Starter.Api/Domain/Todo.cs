namespace Starter.Api.Domain;

/// <summary>A single thing someone wants to get done. The entity never leaves the API; clients see <c>TodoResponse</c>.</summary>
public sealed class Todo
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    /// <summary>Which team owns the todo. One team must never see or change another team's todos.</summary>
    public int TeamId { get; set; }

    /// <summary>Soft-delete flag: a hidden todo is kept in the database but left out of the lists.</summary>
    public bool IsDeleted { get; set; }

    public bool IsDone { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>The last day the todo may be finished on, in Swedish time. A date, not a timestamp: "by Friday" means all of Friday.</summary>
    public DateOnly? DueDate { get; set; }

    public Priority Priority { get; set; } = Priority.Normal;

    /// <summary>When the todo was marked done. Needed to measure whether deadlines are met.</summary>
    public DateTimeOffset? CompletedAt { get; set; }
}
