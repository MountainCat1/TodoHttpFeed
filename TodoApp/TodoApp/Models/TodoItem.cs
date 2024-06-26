﻿namespace TodoApp.Models;

public class TodoItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime LastModifieDateTime { get; set; }
}