namespace FsharpTodoApp.Infrastructure.Features.Todo.Repositories

open System.Linq
open Microsoft.EntityFrameworkCore
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Infrastructure.Persistence.Repositories
open FsharpTodoApp.Infrastructure.Features.Todo.DataModels
open FsharpTodoApp.Domain.Features.Todo.Enums
open FsharpTodoApp.Domain.Common.Entities
open FsToolkit.ErrorHandling

type TodoRepository(ctx: FsharpTodoApp.Infrastructure.Persistence.AppDbContext) =
    interface ITodoRepository with
        member _.GetById(_, id) =
            ctx.Todos
                .Where(fun x -> x.PublicId = id)
                .Select(fun e ->
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
                      Reviewer = e.Reviewer |> TodoReviewer.hydrate })
                .SingleOrDefaultAsync()
            |> Task.map Option.ofObj

        member _.Save(_, entity) =
            { EntityBase = entity.Base
              Query = ctx.Todos
              Dehydrate = fun d -> entity |> TodoDataModel.dehydrate d
              AfterSave = None }
            |> RepositoryHelper.save ctx
            |> Task.map (fun newBase -> { entity with Base = newBase })
