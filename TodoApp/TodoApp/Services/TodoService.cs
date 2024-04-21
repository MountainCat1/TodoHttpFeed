﻿using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Services;

public interface ITodoItemService
{
    Task<List<TodoItem>> GetTodosAsync();
    Task<TodoItem> AddTodoAsync(string createDtoTitle, string createDtoDescription);

    Task<ICollection<TodoItem>> GetFeedAsync(
        Guid? lastTodoId,
        int count,
        int timeout = 5,
        CancellationToken ct = default);
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
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task<ICollection<TodoItem>> GetFeedAsync(
        Guid? lastTodoId,
        int count,
        int timeout = 5,
        CancellationToken ct = default)
    {
        if (lastTodoId == null)
        {
            return await _context.TodoItems
                .OrderBy(t => t.CreatedDateTime)
                .Take(count)
                .ToListAsync(ct);
        }

        var referenceItem = await _context.TodoItems
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
                    var newItems = await _context.TodoItems
                        .Where(t => t.CreatedDateTime > referenceItem.CreatedDateTime)
                        .OrderBy(t => t.CreatedDateTime)
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
                if (ct.IsCancellationRequested)
                {
                    throw;
                }
            }
        }

        return new List<TodoItem>();
    }
}