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

    let handleAsync deps (actor, id) =
        asyncResult {
            let! entity =
                deps.Repository.GetByIdAsync(Some actor, id)
                |> AsyncResult.requireSomeAsync NotFoundError

            let! updated = entity |> TodoPolicyService.buildDeleted deps.PolicyDeps actor

            do! deps.Repository.SaveAsync(actor, updated) |> Async.Ignore
        }
