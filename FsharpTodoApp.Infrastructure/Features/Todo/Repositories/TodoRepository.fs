namespace FsharpTodoApp.Infrastructure.Features.Todo.Repositories

open System.Linq
open Microsoft.EntityFrameworkCore
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Infrastructure.Persistence.Utils
open FsharpTodoApp.Infrastructure.Features.Todo.DataModels

module private TodoMapHelper =
    open FsharpTodoApp.Domain.Common.Entities
    open FsharpTodoApp.Domain.Features.Todo.ValueObjects
    open FsharpTodoApp.Application.Features.Todo.Enums

    let domainMapExpression =
        <@
            fun (e: TodoDataModel) ->
                { Base =
                    EntityBase.recreate
                        (e.Id, e.PublicId)
                        (e.CreatedAt, e.CreatedBy)
                        (e.UpdatedAt, e.UpdatedBy)
                        (e.DeletedAt, e.DeletedBy)
                  Title = e.Title |> TodoTitle.recreate
                  Description = e.Description |> TodoDescription.recreate
                  DueDate = e.DueDate |> TodoDueDate.recreate
                  Status = e.Status |> TodoStatusEnum.toDomain |> TodoStatus.recreate
                  Assignee = e.Assignee |> TodoAssignee.recreate
                  Reviewer = e.Reviewer |> TodoReviewer.recreate }
        @>
        |> ExprHelper.toExpression

type TodoRepository(ctx: FsharpTodoApp.Infrastructure.Persistence.AppDbContext) =
    interface ITodoRepository with
        member _.GetByIdAsync(_, id) : Async<TodoEntity option> =
            async {
                let! todo =
                    ctx.Todos
                        .Where(fun x -> x.PublicId = id)
                        .Select(TodoMapHelper.domainMapExpression)
                        .SingleOrDefaultAsync()
                    |> Async.AwaitTask

                return Option.ofObj todo
            }

        member _.SaveAsync(_, entity) : Async<TodoEntity> =
            async {
                let! newBase =
                    RepositoryHelper.saveAsync
                        ctx
                        (fun ctx -> ctx.Todos) // Include any navigation properties if needed
                        entity.Base
                        (fun e -> e |> TodoDataModel.fromDomain entity)
                        None

                return { entity with Base = newBase }
            }
