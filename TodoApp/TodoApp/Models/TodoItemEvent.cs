namespace TodoApp.Models;

public class TodoItemEvent
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public EventMethods Method { get; set; } 
    public DateTime Time { get; set; }
    public string Data { get; set; }
    public string Subject { get; set; }
}

public enum EventMethods
{
    Create,
    Update,
    Delete
}