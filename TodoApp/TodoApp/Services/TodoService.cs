using TodoApp.Models;

namespace TodoApp.Services;

public interface ITodoService
{
    Task<List<Todo>> GetTodosAsync();
    void AddTodo(string createDtoTitle, string createDtoDescription);
}

public class TodoService : ITodoService
{
    private static readonly List<Todo> Todos = new();

    public void AddTodo(Todo todo)
    {
        Todos.Add(todo);
    }
    
    public Task<List<Todo>> GetTodosAsync()
    {
        return Task.FromResult(Todos);
    }

    public void AddTodo(string createDtoTitle, string createDtoDescription)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = createDtoTitle,
            Description = createDtoDescription
        };

        Todos.Add(todo);
    }
}