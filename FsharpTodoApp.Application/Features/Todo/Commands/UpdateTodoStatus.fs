namespace FsharpTodoApp.Application.Features.Todo.Commands

module UpdateTodoStatus =
    open FsharpTodoApp.Application.Common.Utils
    open FsharpTodoApp.Application.Features.Todo.Dtos.Commands
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsToolkit.ErrorHandling
    open FsharpTodoApp.Application.Features.Todo.Enums

    type Dependencies =
        { Repository: ITodoRepository
          PolicyDeps: TodoPolicyService.Dependencies }

    let handleAsync deps (actor, id, command: TodoUpdateStatusCommandDto) =
        asyncResult {
            let! entity =
                deps.Repository.GetByIdAsync(Some actor, id)
                |> AsyncResult.requireSomeAsync NotFoundError

            let newStatus = TodoStatusEnum.toDomain command.Status
            let! updated = entity |> TodoPolicyService.buildStatusUpdated deps.PolicyDeps actor newStatus

            do! deps.Repository.SaveAsync(actor, updated) |> Async.Ignore
        }
