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
        
        var todoItem = mb.Entity<TodoItem>().ToTable("TodoItems");
        todoItem.HasKey(x => x.Id);
        
        todoItem.Property(x => x.Title).IsRequired().HasMaxLength(128);
        
        todoItem.Property(x => x.Description).HasMaxLength(512);
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}
