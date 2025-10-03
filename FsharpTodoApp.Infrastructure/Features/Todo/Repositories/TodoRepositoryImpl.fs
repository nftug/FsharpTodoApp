namespace FsharpTodoApp.Infrastructure.Features.Todo.Repositories

module TodoRepositoryImpl =
    open System.Linq
    open Microsoft.EntityFrameworkCore
    open FsharpTodoApp.Domain.Features.Todo.Entities
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.ValueObjects
    open FsharpTodoApp.Infrastructure.Persistence.Repositories
    open FsharpTodoApp.Infrastructure.Features.Todo.DataModels
    open FsharpTodoApp.Domain.Common.Entities
    open FsToolkit.ErrorHandling
    open FsharpTodoApp.Persistence

    let private getTodoById (ctx: AppDbContext) _ id =
        ctx.Todos
            .Where(fun x -> x.PublicId = id)
            .Select(fun e ->
                { Base =
                    EntityBase.hydrate
                        (e.Id, e.PublicId)
                        (e.CreatedAt, e.CreatedBy)
                        (e.UpdatedAt |> Option.ofNullable, e.UpdatedBy |> Option.ofObj)
                        (e.DeletedAt |> Option.ofNullable, e.DeletedBy |> Option.ofObj)
                  Title = e.Title |> TodoTitle.hydrate
                  Description = e.Description |> Option.ofObj |> TodoDescription.hydrate
                  DueDate = e.DueDate |> Option.ofNullable |> TodoDueDate.hydrate
                  Status = e.Status |> TodoStatus.fromEnum |> TodoStatus.hydrate
                  Assignee = e.Assignee |> Option.ofObj |> TodoAssignee.hydrate
                  Reviewer = e.Reviewer |> Option.ofObj |> TodoReviewer.hydrate })
            .SingleOrDefaultAsync()
        |> Task.map Option.ofObj

    let private saveTodo (ctx: AppDbContext) _ entity =
        { EntityBase = entity.Base
          Query = ctx.Todos
          Dehydrate = fun d -> entity |> TodoDataModel.dehydrate d
          AfterSave = None }
        |> RepositoryHelper.save ctx
        |> Task.map (fun newBase -> { entity with Base = newBase })

    let create (ctx: AppDbContext) : TodoRepository =
        { GetTodoById = getTodoById ctx
          SaveTodo = saveTodo ctx }
