using Microsoft.EntityFrameworkCore;
using Starter.Api.Domain;

namespace Starter.Api.Data;

/// <summary>
/// Starter data so every team has something to look at. Four teams share one database and one app,
/// which is the whole point of week 3: the requirement is that no team may see another team's todos.
/// </summary>
public static class TeamSeed
{
    // A fixed moment, so the seeded data is identical on every machine.
    private static readonly DateTimeOffset Created = new(2026, 11, 2, 8, 0, 0, TimeSpan.Zero);

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Sälj", Slug = "salj" },
            new Team { Id = 2, Name = "Ekonomi", Slug = "ekonomi" },
            new Team { Id = 3, Name = "Support", Slug = "support" },
            new Team { Id = 4, Name = "Utveckling", Slug = "utveckling" });

        modelBuilder.Entity<Todo>().HasData(
            Todo(101, 1, "Ring tillbaka till Volvo om offerten", Priority.High, new DateOnly(2026, 12, 1)),
            Todo(102, 1, "Uppdatera pipeline i CRM", Priority.Normal, null),
            Todo(103, 1, "Boka lunch med nytt prospekt", Priority.Low, new DateOnly(2026, 12, 5)),

            // Ekonomi har saker som inga andra team ska se.
            Todo(201, 2, "Lönekörning december – granska innan utbetalning", Priority.High, new DateOnly(2026, 12, 20)),
            Todo(202, 2, "Bonusunderlag ledningsgrupp (konfidentiellt)", Priority.High, new DateOnly(2026, 12, 15)),
            Todo(203, 2, "Byt lösenord till bankportalen", Priority.Normal, null),

            Todo(301, 3, "Svara på ärende #4812", Priority.Normal, new DateOnly(2026, 11, 20)),
            Todo(302, 3, "Skriv FAQ om nya inloggningen", Priority.Low, null),

            Todo(401, 4, "Code review på PR #37", Priority.Normal, new DateOnly(2026, 11, 18)),
            Todo(402, 4, "Uppgradera till .NET 10", Priority.Low, null));
    }

    private static Todo Todo(int id, int teamId, string title, Priority priority, DateOnly? dueDate) => new()
    {
        Id = id,
        TeamId = teamId,
        Title = title,
        Priority = priority,
        DueDate = dueDate,
        CreatedAt = Created,
        IsDone = false,
        IsDeleted = false,
        CompletedAt = null,
    };
}
