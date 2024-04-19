using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Controllers;

[ApiController]
[Route("todo")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        var todos = await _todoService.GetTodosAsync();

        return Ok(todos);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddTodo([FromBody] Todo todo)
    {
        _todoService.AddTodo(todo);

        return Ok();
    }
}