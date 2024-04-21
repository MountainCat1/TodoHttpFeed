using TodoApp.Models;

namespace TodoApp.Dtos;

public class TodoFeedResponse
{
    public ICollection<TodoItem> Data { get; set; }
    public int Count { get; set; }
    public string LastTodoId { get; set; }
    public string Next { get; set; }
}