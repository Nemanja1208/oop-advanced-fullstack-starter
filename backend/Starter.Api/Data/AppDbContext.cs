using Microsoft.EntityFrameworkCore;
using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>(todo =>
        {
            todo.HasKey(t => t.Id);
            todo.Property(t => t.Title).HasMaxLength(TodoRules.MaxTitleLength).IsRequired();
        });
    }
}
