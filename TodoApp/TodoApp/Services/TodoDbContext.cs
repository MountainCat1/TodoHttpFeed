using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Services;

public class TodoDbContext : DbContext
{
    protected TodoDbContext()
    {
    }

    public TodoDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // TodoItem
        var todoItem = mb.Entity<TodoItem>().ToTable("TodoItems");
        todoItem.HasKey(x => x.Id);

        todoItem.Property(x => x.Title)
            .IsRequired().HasMaxLength(128);

        todoItem.Property(x => x.Description)
            .HasMaxLength(512);

        todoItem.Property(x => x.CreatedDateTime)
            .IsRequired();


        // TodoItemEvent
        var todoItemEntity = mb.Entity<TodoItemEvent>();

        todoItemEntity.HasKey(x => x.Id);

        todoItemEntity.Property(x => x.Type)
            .HasMaxLength(64);

        todoItemEntity.Property(x => x.Time)
            .IsRequired();

        todoItemEntity.Property(x => x.Method)
            .HasConversion<string>()
            .IsRequired();

        todoItemEntity.Property(x => x.Data)
            .IsRequired(false);
    }


    public DbSet<TodoItem> TodoItems { get; set; } = null!;
    public DbSet<TodoItemEvent> TodoItemEvents { get; set; } = null!;
}