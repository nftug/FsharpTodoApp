using Microsoft.EntityFrameworkCore;

namespace FsharpTodoApp.Persistence.DataModels;

public abstract class DataModelBase
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }


    public static void OnModelCreating<T>(ModelBuilder modelBuilder, string tableName)
        where T : DataModelBase
    {
        modelBuilder.Entity<T>().HasKey(e => e.Id);

        modelBuilder.Entity<T>().HasIndex(e => e.PublicId).IsUnique();

        modelBuilder.Entity<T>().HasQueryFilter(e => e.DeletedAt == null);

        modelBuilder.Entity<T>().ToTable(tableName);
    }
}
