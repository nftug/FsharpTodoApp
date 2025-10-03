namespace FsharpTodoApp.Infrastructure.Persistence

open Microsoft.EntityFrameworkCore
open FsharpTodoApp.Infrastructure.Features.Todo.DataModels
open FsharpTodoApp.Infrastructure.Features.User.DataModels

type AppDbContext(options) =
    inherit DbContext(options)

    override _.OnConfiguring optionsBuilder =
        optionsBuilder.UseQueryTrackingBehavior QueryTrackingBehavior.NoTracking
        |> ignore

        base.OnConfiguring optionsBuilder

    [<DefaultValue>]
    val mutable Todos: DbSet<TodoDataModel>

    [<DefaultValue>]
    val mutable Users: DbSet<UserDataModel>

    override _.OnModelCreating modelBuilder =
        TodoDataModel.onModelCreating modelBuilder
        UserDataModel.onModelCreating modelBuilder
        base.OnModelCreating modelBuilder
