using BenchmarkDotNet.Attributes;
using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Benchmarks;

/// <summary>
/// Measures the kravspec limit "sorting 1 000 todos takes under 1 ms". The data is generated with a fixed seed,
/// so every run measures exactly the same list.
/// </summary>
[MemoryDiagnoser]
public class WorkListOrderingBenchmarks
{
    private static readonly DateOnly Today = new(2026, 11, 16);
    private List<Todo> _todos = [];

    [Params(100, 1_000, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _todos = Enumerable.Range(1, Count)
            .Select(id => new Todo
            {
                Id = id,
                Title = $"Uppgift {id}",
                IsDone = random.Next(4) == 0,
                DueDate = random.Next(5) == 0 ? null : Today.AddDays(random.Next(-10, 30)),
                Priority = (Priority)random.Next(3),
            })
            .ToList();
    }

    [Benchmark]
    public IReadOnlyList<Todo> ForWorkList() => TodoOrdering.ForWorkList(_todos, Today);
}
