namespace FsharpTodoApp.Infrastructure.Features.Todo.Repositories

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

module TodoRepositoryImpl =
    let private getTodoById (ctx: AppDbContext) _ id =
        ctx.Todos
            .Where(fun x -> x.PublicId = id)
            .Select(fun x ->
                { Base =
                    EntityBase.hydrate
                        (x.Id, x.PublicId)
                        (x.CreatedAt, x.CreatedBy)
                        (Option.ofNullable x.UpdatedAt, Option.ofObj x.UpdatedBy)
                        (Option.ofNullable x.DeletedAt, Option.ofObj x.DeletedBy)
                  Title = TodoTitle.hydrate x.Title
                  Description = TodoDescription.hydrate (Option.ofObj x.Description)
                  DueDate = TodoDueDate.hydrate (Option.ofNullable x.DueDate)
                  Status = TodoStatus.hydrate x.Status
                  Assignee = TodoAssignee.hydrate (Option.ofObj x.Assignee)
                  Reviewer = TodoReviewer.hydrate (Option.ofObj x.Reviewer) })
            .SingleOrDefaultAsync()
        |> Task.map Option.ofObj

    let private saveTodo (ctx: AppDbContext) _ entity =
        { EntityBase = entity.Base
          Query = ctx.Todos
          Dehydrate = fun d -> entity |> TodoDataModelHelper.dehydrate d
          AfterSave = None }
        |> RepositoryHelper.save ctx
        |> Task.map (fun newBase -> { entity with Base = newBase })

    let create (ctx: AppDbContext) : TodoRepository =
        { GetTodoById = getTodoById ctx
          SaveTodo = saveTodo ctx }
