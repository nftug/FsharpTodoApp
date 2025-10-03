namespace FsharpTodoApp.Application.Features.Todo.Commands

open FsharpTodoApp.Application.Features.Todo.Dtos.Commands
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Domain.Features.Todo.Services
open FsharpTodoApp.Domain.Features.Todo.Enums
open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Common.ValueObjects

type UpdateTodoStatus =
    { Handle: (Actor * System.Guid * TodoUpdateStatusCommandDto) -> TaskResult<unit, AppError> }

module UpdateTodoStatus =
    let private handle (repo, policyService) (actor, id, command: TodoUpdateStatusCommandDto) =
        taskResult {
            let! entity = repo.GetTodoById (Some actor) id |> TaskResult.requireSome NotFoundError

            let newStatus = TodoStatusEnum.ofDomain command.Status
            let! updated = entity |> policyService.BuildStatusUpdated actor newStatus

            do! repo.SaveTodo actor updated |> Task.ignore
        }

    let create repo policyService =
        { Handle = handle (repo, policyService) }
