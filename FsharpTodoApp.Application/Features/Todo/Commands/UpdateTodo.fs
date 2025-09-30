namespace FsharpTodoApp.Application.Features.Todo.Commands

module UpdateTodo =
    open FsharpTodoApp.Application.Common.Utils
    open FsharpTodoApp.Application.Features.Todo.Dtos.Commands
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsharpTodoApp.Domain.Features.User.Interfaces
    open FsToolkit.ErrorHandling

    type Dependencies =
        { Repository: ITodoRepository
          UserRef: IUserReferenceService
          PolicyDeps: TodoPolicyService.Dependencies }

    let handleAsync deps (actor, id, command) =
        asyncResult {
            let! entity =
                deps.Repository.GetByIdAsync(Some actor, id)
                |> AsyncResult.requireSomeAsync NotFoundError

            let! assignee =
                deps.UserRef.GetByUserNameAsync
                |> AsyncResult.maybeFetchAsync (ValidationError "Could not find assignee user")
                <| command.AssigneeUserName

            let! reviewer =
                deps.UserRef.GetByUserNameAsync
                |> AsyncResult.maybeFetchAsync (ValidationError "Could not find reviewer user")
                <| command.ReviewerUserName

            let! updated =
                TodoPolicyService.buildUpdated
                    deps.PolicyDeps
                    actor
                    (command.Title, command.Description, command.DueDate, assignee, reviewer)
                    entity

            do! deps.Repository.SaveAsync(actor, updated) |> Async.Ignore
        }
