namespace FsharpTodoApp.Application.Features.Todo.Commands

module UpdateTodoStatus =
    open FsharpTodoApp.Application.Features.Todo.Dtos.Commands
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsToolkit.ErrorHandling
    open FsharpTodoApp.Application.Features.Todo.Enums

    type Dependencies =
        { Repository: ITodoRepository
          PolicyDeps: TodoPolicyService.Dependencies }

    let handle deps (actor, id, command: TodoUpdateStatusCommandDto) =
        taskResult {
            let! entity = deps.Repository.GetById(Some actor, id) |> TaskResult.requireSome NotFoundError

            let newStatus = TodoStatusEnum.ofDomain command.Status
            let! updated = entity |> TodoPolicyService.buildStatusUpdated deps.PolicyDeps actor newStatus

            do! deps.Repository.Save(actor, updated) |> Task.ignore
        }
