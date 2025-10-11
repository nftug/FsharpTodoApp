using FsharpTodoApp.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FsharpTodoApp.Persistence.DataModels;

public class UserDataModel : DataModelBase
{
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public ActorRoleEnum[] Roles { get; set; } = [];

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreating<UserDataModel>(modelBuilder, "Users");

        modelBuilder.Entity<UserDataModel>()
            .HasIndex(e => e.UserName)
            .IsUnique();

        modelBuilder.Entity<UserDataModel>()
            .Property(e => e.Roles)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => Enum.Parse<ActorRoleEnum>(r)).ToArray(),
                new ValueComparer<ActorRoleEnum[]>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray())
            );
    }
}
