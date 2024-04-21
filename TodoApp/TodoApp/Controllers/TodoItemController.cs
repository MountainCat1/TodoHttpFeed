using Microsoft.AspNetCore.Mvc;
using TodoApp.Dtos;
using TodoApp.Services;

namespace TodoApp.Controllers;

[ApiController]
[Route("api/todo-items")]
public class TodoItemController : ControllerBase
{
    private readonly ITodoItemService _todoItemService;

    public TodoItemController(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        var todos = await _todoItemService.GetTodosAsync();

        return Ok(todos);
    }

    [HttpPost]
    public async Task<IActionResult> AddTodo([FromBody] TodoItemCreateDto itemCreateDto)
    {
        await _todoItemService.AddTodoAsync(itemCreateDto.Title, itemCreateDto.Description);

        return Ok();
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] Guid? lastTodoId = null,
        [FromQuery] int timeout = 60,
        [FromQuery] int count = 5,
        CancellationToken ct = default)
    {
        var todos = await _todoItemService.GetFeedAsync(lastTodoId, count, timeout, ct);

        var response = new TodoFeedResponse()
        {
            Data = todos,
            Count = todos.Count,
            LastTodoId = todos.LastOrDefault()?.Id.ToString() ?? "none",
            Next = Url.Action("GetFeed", new { lastTodoId = todos.LastOrDefault()?.Id, count = count })
        };

        HttpContext.Response.ContentType = "application/cloudevents-batch+json";
        return Ok(response);
    }
}