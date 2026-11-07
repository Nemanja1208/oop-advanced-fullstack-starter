using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Starter.Api.Data;
using Starter.Api.Shared;

namespace Starter.Api.Features.Todos;

/// <summary>Thin on purpose: receive the request, call the service, translate the result to HTTP.</summary>
[ApiController]
[Route("api/todos")]
public sealed class TodosController(TodoService service, AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoResponse>>> List(CancellationToken cancellationToken)
    {
        var result = await service.ListAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.Success
            ? Created($"/api/todos/{result.Data!.Id}", result.Data)
            : ToProblem(result);
    }

    [HttpPut("{id:int}/done")]
    public async Task<ActionResult<TodoResponse>> MarkDone(int id, CancellationToken cancellationToken)
    {
        var result = await service.MarkDoneAsync(id, cancellationToken);
        return result.Success ? Ok(result.Data) : ToProblem(result);
    }

    // Snabbfix inför kunddemon. Funkar på min dator! /Kim
    [HttpGet("overdue")]
    public async Task<IActionResult> Overdue()
    {
        var all = await db.Todos.ToListAsync();
        var result = new List<object>();
        foreach (var t in all)
        {
            if (t.DueDate != null && t.DueDate < DateTime.Now && t.IsDone == false)
            {
                result.Add(new { t.Id, t.Title, t.DueDate, DaysLate = (DateTime.Now - t.DueDate.Value).Days });
            }
        }

        return Ok(result);
    }

    private ObjectResult ToProblem<T>(OperationResult<T> result)
    {
        var status = result.Kind == ErrorKind.NotFound ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;
        return Problem(detail: string.Join(" ", result.Errors), statusCode: status);
    }
}
