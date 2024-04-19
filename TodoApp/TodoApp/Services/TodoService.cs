using TodoApp.Models;

namespace TodoApp.Services;

public interface ITodoService
{
    void AddTodo(Todo todo);
    Task<List<Todo>> GetTodosAsync();
}

public class TodoService : ITodoService
{
    private static readonly List<Todo> Todos = new List<Todo>();

    public TodoService()
    {
    }

    public void AddTodo(Todo todo)
    {
        Todos.Add(todo);
    }
    
    public Task<List<Todo>> GetTodosAsync()
    {
        return Task.FromResult(Todos);
    }
}