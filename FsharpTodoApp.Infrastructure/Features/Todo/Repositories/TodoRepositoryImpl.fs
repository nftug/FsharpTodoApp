namespace FsharpTodoApp.Infrastructure.Features.Todo.Repositories

open System.Linq
open Microsoft.EntityFrameworkCore
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Infrastructure.Persistence.Repositories
open FsharpTodoApp.Infrastructure.Features.Todo.DataModels
open FsharpTodoApp.Domain.Common.Entities
open FsToolkit.ErrorHandling
open FsharpTodoApp.Persistence
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.ValueObjects

module TodoRepositoryImpl =
    let private getTodoById (ctx: AppDbContext) _ id =
        ctx.Todos
            .Where(fun x -> x.PublicId = id)
            .Select(fun x ->
                TodoEntity.hydrate (
                    EntityBase.hydrate (
                        x.Id,
                        x.PublicId,
                        CreatedAudit.hydrate (x.CreatedAt, x.CreatedBy),
                        UpdatedAudit.hydrate (Option.ofNullable x.UpdatedAt, Option.ofObj x.UpdatedBy),
                        DeletedAudit.hydrate (Option.ofNullable x.DeletedAt, Option.ofObj x.DeletedBy)
                    ),
                    TodoTitle.hydrate x.Title,
                    TodoDescription.hydrate (Option.ofObj x.Description),
                    TodoDueDate.hydrate (Option.ofNullable x.DueDate),
                    TodoStatus.hydrate x.Status,
                    TodoAssignee.hydrate (Option.ofObj x.Assignee),
                    TodoReviewer.hydrate (Option.ofObj x.Reviewer)
                ))
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
