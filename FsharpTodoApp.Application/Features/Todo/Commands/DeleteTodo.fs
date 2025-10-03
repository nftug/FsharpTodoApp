namespace FsharpTodoApp.Application.Features.Todo.Commands

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Domain.Features.Todo.Services
open FsToolkit.ErrorHandling

type DeleteTodo =
    { Handle: (Actor * System.Guid) -> TaskResult<unit, AppError> }

module DeleteTodo =
    let private handle repo policyService (actor, id) =
        taskResult {
            let! entity = repo.GetTodoById (Some actor) id |> TaskResult.requireSome NotFoundError

            let! updated = entity |> policyService.BuildDeleted actor

            do! repo.SaveTodo actor updated |> Task.ignore
        }

    let create repo policyService = { Handle = handle repo policyService }
