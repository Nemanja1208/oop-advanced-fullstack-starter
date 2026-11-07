using Microsoft.AspNetCore.Mvc;
using Starter.Api.Shared;

namespace Starter.Api.Features.Todos;

/// <summary>Thin on purpose: receive the request, call the service, translate the result to HTTP.</summary>
[ApiController]
[Route("api/todos")]
public sealed class TodosController(TodoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoResponse>>> List(CancellationToken cancellationToken)
    {
        var result = await service.ListAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IReadOnlyList<OverdueTodoResponse>>> Overdue(CancellationToken cancellationToken)
    {
        var result = await service.ListOverdueAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DeadlineStatsResponse>> Stats(CancellationToken cancellationToken)
    {
        var result = await service.StatsAsync(cancellationToken);
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

    private ObjectResult ToProblem<T>(OperationResult<T> result)
    {
        var status = result.Kind == ErrorKind.NotFound ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;
        return Problem(detail: string.Join(" ", result.Errors), statusCode: status);
    }
}
