using TodoApp.Models;

namespace TodoApp.Services;

public interface ITodoItemService
{
    Task<List<TodoItem>> GetTodosAsync();
    void AddTodo(string createDtoTitle, string createDtoDescription);
}

public class TodoItemService : ITodoItemService
{
    private static readonly List<TodoItem> Todos = new();

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
}