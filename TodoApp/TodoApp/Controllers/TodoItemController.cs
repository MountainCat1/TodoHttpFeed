using Microsoft.AspNetCore.Mvc;
using TodoApp.Dtos;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Controllers;

[ApiController]
[Route("todo")]
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
        _todoItemService.AddTodo(itemCreateDto.Title, itemCreateDto.Description);

        return Ok();
    }
}