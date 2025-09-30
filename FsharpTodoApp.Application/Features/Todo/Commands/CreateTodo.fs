namespace FsharpTodoApp.Application.Features.Todo.Commands

module CreateTodo =
    open FsharpTodoApp.Application.Common.Utils
    open FsharpTodoApp.Application.Common.Dtos
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

    let handleAsync deps (actor, command) =
        asyncResult {
            let! assignee =
                deps.UserRef.GetByUserNameAsync
                |> AsyncResult.maybeFetchAsync (ValidationError "Could not find assignee user")
                <| command.AssigneeUserName

            let! reviewer =
                deps.UserRef.GetByUserNameAsync
                |> AsyncResult.maybeFetchAsync (ValidationError "Could not find reviewer user")
                <| command.ReviewerUserName

            let! entity =
                TodoPolicyService.buildCreated
                    deps.PolicyDeps
                    actor
                    (command.Title, command.Description, command.DueDate, assignee, reviewer)

            return!
                deps.Repository.SaveAsync(actor, entity)
                |> Async.map (fun x -> { ItemId = x.Base.IdSet.PublicId })
        }
