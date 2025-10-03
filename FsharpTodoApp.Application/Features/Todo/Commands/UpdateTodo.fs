namespace FsharpTodoApp.Application.Features.Todo.Commands

open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Domain.Features.Todo.Services
open FsharpTodoApp.Domain.Features.User.Interfaces
open FsharpTodoApp.Application.Features.Todo.Dtos.Commands
open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Common.ValueObjects

type UpdateTodo =
    { Handle: (Actor * System.Guid * TodoUpdateCommandDto) -> TaskResult<unit, AppError> }

module UpdateTodo =
    let private handle (repo, userRef, policySvc) (actor, id, command: TodoUpdateCommandDto) =
        taskResult {
            let! entity = repo.GetById(Some actor, id) |> TaskResult.requireSome NotFoundError

            let! assignee =
                match command.AssigneeUserName with
                | Some arg ->
                    userRef.GetByUserName arg
                    |> TaskResult.requireSome (ValidationError "Could not find assignee user")
                    |> TaskResult.map Some
                | None -> taskResult { return None }

            let! reviewer =
                match command.ReviewerUserName with
                | Some arg ->
                    userRef.GetByUserName arg
                    |> TaskResult.requireSome (ValidationError "Could not find reviewer user")
                    |> TaskResult.map Some
                | None -> taskResult { return None }

            let! updated =
                policySvc.BuildUpdated
                    actor
                    (command.Title, command.Description, command.DueDate, assignee, reviewer)
                    entity

            do! repo.Save(actor, updated) |> Task.ignore
        }

    let create repo userRef policySvc =
        { Handle = handle (repo, userRef, policySvc) }
