namespace Starter.Api.Domain;

/// <summary>A single thing someone wants to get done. The entity never leaves the API; clients see <c>TodoResponse</c>.</summary>
public sealed class Todo
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }
}
