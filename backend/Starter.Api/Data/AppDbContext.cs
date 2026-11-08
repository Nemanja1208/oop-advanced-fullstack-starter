using Microsoft.EntityFrameworkCore;
using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();

    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>(team =>
        {
            team.HasKey(t => t.Id);
            team.Property(t => t.Name).HasMaxLength(100).IsRequired();
            team.Property(t => t.Slug).HasMaxLength(50).IsRequired();
            team.HasIndex(t => t.Slug).IsUnique();
        });

        modelBuilder.Entity<Todo>(todo =>
        {
            todo.HasKey(t => t.Id);
            todo.Property(t => t.Title).HasMaxLength(TodoRules.MaxTitleLength).IsRequired();
            todo.HasIndex(t => t.TeamId);
        });

        TeamSeed.Apply(modelBuilder);
    }
}
