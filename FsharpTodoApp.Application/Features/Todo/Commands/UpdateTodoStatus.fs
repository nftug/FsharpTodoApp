namespace FsharpTodoApp.Application.Features.Todo.Commands

open FsharpTodoApp.Application.Features.Todo.Dtos
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Domain.Features.Todo.Services
open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Common.ValueObjects

type UpdateTodoStatus =
    { Handle: (Actor * System.Guid * TodoUpdateStatusCommandDto) -> TaskResult<unit, AppError> }

module UpdateTodoStatus =
    open FsharpTodoApp.Domain.Features.Todo.ValueObjects

    let private handle (repo, policyService) (actor, id, command: TodoUpdateStatusCommandDto) =
        taskResult {
            let! entity = repo.GetTodoById (Some actor) id |> TaskResult.requireSome NotFoundError

            let newStatus = TodoStatus.fromEnum command.Status
            let! updated = policyService.BuildStatusUpdated actor newStatus entity

            do! repo.SaveTodo actor updated |> Task.ignore
        }

    let create (repo: TodoRepository) (policyService: TodoPolicyService) : UpdateTodoStatus =
        { Handle = handle (repo, policyService) }
