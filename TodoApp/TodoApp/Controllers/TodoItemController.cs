using Microsoft.AspNetCore.Mvc;
using TodoApp.Dtos;
using TodoApp.Models;
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
    
    
    [HttpGet("{todoItemId}")]
    public async Task<IActionResult> GetTodos([FromRoute] Guid todoItemId)
    {
        var todo = await _todoItemService.GetTodoAsync(todoItemId);
        
        if (todo == null)
            return NotFound();

        return Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> AddTodo([FromBody] TodoItemCreateDto itemCreateDto)
    {
        var createdItem = await _todoItemService.AddTodoAsync(itemCreateDto.Title, itemCreateDto.Description);

        return Ok(createdItem);
    }
    
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> DeleteTodo([FromRoute] Guid todoId)
    {
        var deletedItem = await _todoItemService.DeleteTodoAsync(todoId);

        return Ok(deletedItem);
    }
    
    [HttpPut("{todoId}")]
    public async Task<IActionResult> UpdateTodo(
        [FromRoute] Guid todoId,
        [FromBody] TodoItemUpdateDto itemUpdateDto)
    {
        var updatedItem = await _todoItemService.UpdateTodoAsync(todoId, itemUpdateDto.Title, itemUpdateDto.Description);

        return Ok(updatedItem);
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