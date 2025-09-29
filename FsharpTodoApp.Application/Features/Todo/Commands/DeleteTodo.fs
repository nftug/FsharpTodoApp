namespace FsharpTodoApp.Application.Features.Todo.Commands

module DeleteTodo =
    open FsharpTodoApp.Application.Common.Utils
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsToolkit.ErrorHandling

    type Dependencies =
        { Repository: ITodoRepository
          PolicyDeps: TodoPolicyService.Dependencies }

    let handle deps (actor, id) =
        asyncResult {
            let! entity = deps.Repository.GetById(Some actor, id) |> AsyncResult.fromOption NotFoundError

            let! updated = entity |> TodoPolicyService.buildDeletedEntity deps.PolicyDeps actor

            do! deps.Repository.Save(actor, updated) |> Async.Ignore
        }
