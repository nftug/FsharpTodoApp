namespace FsharpTodoApp.Infrastructure.Persistence

open Microsoft.EntityFrameworkCore
open FsharpTodoApp.Infrastructure.Features.Todo.DataModels

type AppDbContext(options) =
    inherit DbContext(options)

    override _.OnConfiguring optionsBuilder =
        optionsBuilder.UseQueryTrackingBehavior QueryTrackingBehavior.NoTracking
        |> ignore

        base.OnConfiguring optionsBuilder

    [<DefaultValue>]
    val mutable Todos: DbSet<TodoDataModel>

    override _.OnModelCreating modelBuilder =
        TodoDataModel.onModelCreating modelBuilder
        base.OnModelCreating modelBuilder
