using TodoApp.Models;

namespace TodoApp.Services;

public interface ITodoItemService
{
    Task<List<TodoItem>> GetTodosAsync();
    void AddTodo(string createDtoTitle, string createDtoDescription);
    Task<TodoItem> GetFeedAsync(Guid lastTodoId, int count, CancellationToken ct = default);
}

public class TodoItemService : ITodoItemService
{
    private static readonly List<TodoItem> Todos = new();
    private readonly TodoDbContext _context;

    public TodoItemService(TodoDbContext context)
    {
        _context = context;
    }

    public void AddTodo(TodoItem todoItem)
    {
        Todos.Add(todoItem);
    }
    
    public Task<List<TodoItem>> GetTodosAsync()
    {
        return Task.FromResult(Todos);
    }

    public void AddTodo(string createDtoTitle, string createDtoDescription)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = createDtoTitle,
            Description = createDtoDescription
        };

        Todos.Add(todo);
    }

    public Task<TodoItem> GetFeedAsync(Guid lastTodoId, int count, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}