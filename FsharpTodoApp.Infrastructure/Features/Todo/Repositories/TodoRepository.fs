namespace FsharpTodoApp.Infrastructure.Features.Todo.Repositories

open System.Linq
open Microsoft.EntityFrameworkCore
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Infrastructure.Persistence.Utils
open FsharpTodoApp.Infrastructure.Features.Todo.DataModels
open FsharpTodoApp.Domain.Common.Entities

module private TodoMapHelper =
    open FsharpTodoApp.Domain.Features.Todo.ValueObjects
    open FsharpTodoApp.Application.Features.Todo.Enums

    let hydrationExpression =
        <@
            fun (e: TodoDataModel) ->
                { Base =
                    EntityBase.hydrate
                        (e.Id, e.PublicId)
                        (e.CreatedAt, e.CreatedBy)
                        (e.UpdatedAt, e.UpdatedBy)
                        (e.DeletedAt, e.DeletedBy)
                  Title = e.Title |> TodoTitle.hydrate
                  Description = e.Description |> TodoDescription.hydrate
                  DueDate = e.DueDate |> TodoDueDate.hydrate
                  Status = e.Status |> TodoStatusEnum.ofDomain |> TodoStatus.hydrate
                  Assignee = e.Assignee |> TodoAssignee.hydrate
                  Reviewer = e.Reviewer |> TodoReviewer.hydrate }
        @>
        |> ExprHelper.toExpression

type TodoRepository(ctx: FsharpTodoApp.Infrastructure.Persistence.AppDbContext) =
    interface ITodoRepository with
        member _.GetByIdAsync(_, id) : Async<TodoEntity option> =
            async {
                let! todo =
                    ctx.Todos
                        .Where(fun x -> x.PublicId = id)
                        .Select(TodoMapHelper.hydrationExpression)
                        .SingleOrDefaultAsync()
                    |> Async.AwaitTask

                return Option.ofObj todo
            }

        member _.SaveAsync(_, entity) : Async<TodoEntity> =
            async {
                let! newBase =
                    { EntityBase = entity.Base
                      Query = ctx.Todos
                      Dehydrate = fun d -> entity |> TodoDataModel.dehydrate d
                      AfterSave = None }
                    |> RepositoryHelper.saveAsync ctx

                return { entity with Base = newBase }
            }
