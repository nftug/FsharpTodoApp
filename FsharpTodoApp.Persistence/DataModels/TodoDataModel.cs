using FsharpTodoApp.Domain.Features.Todo.Enums;
using Microsoft.EntityFrameworkCore;

namespace FsharpTodoApp.Persistence.DataModels;

public class TodoDataModel : DataModelBase
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TodoStatusEnum Status { get; set; }
    public string? Assignee { get; set; }
    public string? Reviewer { get; set; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreating<TodoDataModel>(modelBuilder, "Todos");

        modelBuilder.Entity<TodoDataModel>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TodoStatusEnum>(v));
    }
}
