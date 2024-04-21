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
    public async Task<IActionResult> GetFeed([FromQuery] Guid lastTodoId, [FromQuery] int count = 5)
    {
        var todos = await _todoItemService.GetFeedAsync(lastTodoId, count);

        return Ok(todos);
    }
}