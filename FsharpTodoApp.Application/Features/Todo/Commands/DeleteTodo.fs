namespace FsharpTodoApp.Application.Features.Todo.Commands

module DeleteTodo =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsToolkit.ErrorHandling

    type Dependencies =
        { Repository: ITodoRepository
          PolicyDeps: TodoPolicyService.Dependencies }

    let handle deps (actor, id) =
        taskResult {
            let! entity = deps.Repository.GetById(Some actor, id) |> TaskResult.requireSome NotFoundError

            let! updated = entity |> TodoPolicyService.buildDeleted deps.PolicyDeps actor

            do! deps.Repository.Save(actor, updated) |> Task.ignore
        }
