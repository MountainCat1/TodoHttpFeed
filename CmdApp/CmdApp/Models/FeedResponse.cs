namespace CmdApp.Models;

public class FeedResponse
{
    public List<TodoItemEvent> Data { get; set; }
    public int Count { get; set; }
    public string LastTodoId { get; set; }
    public string Next { get; set; }
}

public enum EventMethods
{
    Create,
    Update,
    Delete
}