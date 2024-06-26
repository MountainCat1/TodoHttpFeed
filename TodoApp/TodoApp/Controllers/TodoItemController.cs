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
    
    [HttpGet("{todoId}")]
    public async Task<IActionResult> GetTodo([FromRoute] Guid todoId)
    {
        var todo = await _todoItemService.GetTodoAsync(todoId);
        
        if (todo == null)
            return NotFound();

        return Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> AddTodo([FromBody] TodoItemCreateDto itemCreateDto)
    {
        await _todoItemService.AddTodoAsync(itemCreateDto.Title, itemCreateDto.Description);

        return Ok();
    }
    
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> DeleteTodo([FromRoute] Guid todoId)
    {
        await _todoItemService.DeleteTodoAsync(todoId);

        return Ok();
    }
    
    [HttpPut("{todoId}")]
    public async Task<IActionResult> UpdateTodo(
        [FromRoute] Guid todoId,
        [FromBody] TodoItemUpdateDto itemUpdateDto)
    {
        await _todoItemService.UpdateTodoAsync(todoId, itemUpdateDto.Title, itemUpdateDto.Description);

        return Ok();
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] Guid? lastEventId = null,
        [FromQuery] int timeout = 60,
        [FromQuery] int count = 5,
        CancellationToken ct = default)
    {
        var todos = await _todoItemService.GetFeedAsync(lastEventId, count, timeout, ct);
        
        HttpContext.Response.ContentType = "application/cloudevents-batch+json";
        return Ok(todos);
    }
}