using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Services;

public interface ITodoItemService
{
    Task<List<TodoItem>> GetTodosAsync();
    Task<TodoItem> AddTodoAsync(string createDtoTitle, string createDtoDescription);
    Task<TodoItem> DeleteTodoAsync(Guid todoId);
    Task<TodoItem> UpdateTodoAsync(Guid todoId, string updateDtoTitle, string updateDtoDescription);

    Task<ICollection<TodoItemEvent>> GetFeedAsync(
        Guid? lastTodoId,
        int count,
        int timeout = 5,
        CancellationToken ct = default);

    Task<TodoItem?> GetTodoAsync(Guid todoId);
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
            Description = createDtoDescription,
            CreatedDateTime = DateTime.UtcNow,
            LastModifieDateTime = DateTime.UtcNow
        };

        _context.TodoItems.Add(todo);
        _context.TodoItemEvents.Add(CreateEvent(todo, EventMethods.Create));
        
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task<TodoItem> DeleteTodoAsync(Guid todoId)
    {
        var todo = await _context.TodoItems.FindAsync(todoId);
        if (todo == null)
            throw new ArgumentException("Invalid todo item", nameof(todoId));

        _context.TodoItems.Remove(todo);
        _context.TodoItemEvents.Add(CreateEvent(todo, EventMethods.Delete));
        
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task<TodoItem> UpdateTodoAsync(Guid todoId, string updateDtoTitle, string updateDtoDescription)
    {
        var todo = await _context.TodoItems.FindAsync(todoId);
        if (todo == null)
            throw new ArgumentException("Invalid todo item", nameof(todoId));

        todo.Title = updateDtoTitle;
        todo.Description = updateDtoDescription;
        todo.LastModifieDateTime = DateTime.UtcNow;

        _context.TodoItems.Update(todo);
        _context.TodoItemEvents.Add(CreateEvent(todo, EventMethods.Update));
        
        await _context.SaveChangesAsync();

        return todo;
    }

    public async Task<ICollection<TodoItemEvent>> GetFeedAsync(
        Guid? lastTodoId,
        int count,
        int timeout = 5,
        CancellationToken ct = default)
    {
        if (lastTodoId == null)
        {
            return await _context.TodoItemEvents
                .OrderBy(t => t.Time)
                .Take(count)
                .ToListAsync(ct);
        }

        var referenceItem = await _context.TodoItemEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == lastTodoId, cancellationToken: ct);

        if (referenceItem == null)
            throw new ArgumentException("Invalid reference item", nameof(lastTodoId));

        using (var timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct))
        {
            timeoutCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(timeout));

            try
            {
                while (!timeoutCancellationTokenSource.IsCancellationRequested)
                {
                    var newItems = await _context.TodoItemEvents
                        .Where(t => t.Time > referenceItem.Time)
                        .OrderBy(t => t.Time)
                        .Take(count)
                        .ToListAsync(timeoutCancellationTokenSource.Token);

                    if (newItems.Any())
                    {
                        return newItems;
                    }

                    await Task.Delay(5000, timeoutCancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // If the operation was canceled, we throw the exception
                // It it wasn't it means the timeout was reached
                if (ct.IsCancellationRequested)
                {
                    throw;
                }
            }
        }

        return new List<TodoItemEvent>();
    }

    public async Task<TodoItem?> GetTodoAsync(Guid todoId)
    {
        return await _context.TodoItems.FindAsync(todoId);
    }

    private static TodoItemEvent CreateEvent(TodoItem todo, EventMethods method)
    {
        if (method == EventMethods.Delete)
        {
            return new TodoItemEvent()
            {
                // We dont add "data" while deleting
                Id = Guid.NewGuid(),
                Method = method,
                Time = DateTime.UtcNow,
                Type = typeof(TodoItem).FullName ?? nameof(TodoItem),
                Subject = todo.Id.ToString()
            };
        }
        
        return new TodoItemEvent()
        {
            Data = JsonSerializer.Serialize(todo),
            Id = Guid.NewGuid(),
            Method = method,
            Time = DateTime.UtcNow,
            Type = typeof(TodoItem).FullName ?? nameof(TodoItem),
            Subject = todo.Id.ToString()
        };
    }
}