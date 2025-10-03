namespace FsharpTodoApp.Application.Features.Todo.Commands

open FsharpTodoApp.Application.Common.Dtos
open FsharpTodoApp.Application.Features.Todo.Dtos.Commands
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Features.Todo.Interfaces
open FsharpTodoApp.Domain.Features.Todo.Services
open FsharpTodoApp.Domain.Features.User.Interfaces
open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Common.ValueObjects

type CreateTodo =
    { Handle: (Actor * TodoCreateCommandDto) -> TaskResult<ItemCreatedResponseDto, AppError> }

module CreateTodo =
    let private handle (repo, userRef, policyService) (actor, command: TodoCreateCommandDto) =
        taskResult {
            let! assignee =
                match command.AssigneeUserName with
                | Some arg ->
                    userRef.GetUserRefByUserName arg
                    |> TaskResult.requireSome (ValidationError "Could not find assignee user")
                    |> TaskResult.map Some
                | None -> taskResult { return None }

            let! reviewer =
                match command.ReviewerUserName with
                | Some arg ->
                    userRef.GetUserRefByUserName arg
                    |> TaskResult.requireSome (ValidationError "Could not find reviewer user")
                    |> TaskResult.map Some
                | None -> taskResult { return None }

            let! entity =
                policyService.BuildCreated
                    actor
                    (command.Title, command.Description, command.DueDate, assignee, reviewer)

            let! created =
                repo.SaveTodo actor entity
                |> Task.map (fun x -> Ok(x.Base |> ItemCreatedResponseDto.create))

            return created
        }

    let create repo userRef policyService =
        { Handle = handle (repo, userRef, policyService) }
