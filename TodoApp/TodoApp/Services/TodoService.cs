using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Services;

public interface ITodoItemService
{
    Task<List<TodoItem>> GetTodosAsync();
    Task<TodoItem> AddTodoAsync(string createDtoTitle, string createDtoDescription);
    Task<TodoItem> GetFeedAsync(Guid lastTodoId, int count, CancellationToken ct = default);
}

public class TodoItemService : ITodoItemService
{
    private readonly TodoDbContext _context;

    public TodoItemService(TodoDbContext context)
    {
        _context = context;
    }
    
    public Task<List<TodoItem>> GetTodosAsync()
    {
        return _context.TodoItems.ToListAsync();
    }

    public async Task<TodoItem> AddTodoAsync(string createDtoTitle, string createDtoDescription)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = createDtoTitle,
            Description = createDtoDescription
        };

        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync();
        return todo;
    }

    public Task<TodoItem> GetFeedAsync(Guid lastTodoId, int count, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}