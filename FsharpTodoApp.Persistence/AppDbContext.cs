using FsharpTodoApp.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;

namespace FsharpTodoApp.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<TodoDataModel> Todos { get; set; } = null!;
    public DbSet<UserDataModel> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        TodoDataModel.OnModelCreating(modelBuilder);
        UserDataModel.OnModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
