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

    let handle deps (actor, id, command) =
        asyncResult {
            let! entity = deps.Repository.GetById(Some actor, id) |> AsyncResult.fromOption NotFoundError

            let! assignee =
                match command.AssigneeUserName with
                | Some userName ->
                    deps.UserRef.GetByUserName userName
                    |> AsyncResult.fromOption (ValidationError "Could not find assignee user")
                    |> AsyncResult.map Some
                | None -> asyncResult { return None }

            let! reviewer =
                match command.ReviewerUserName with
                | Some userName ->
                    deps.UserRef.GetByUserName userName
                    |> AsyncResult.fromOption (ValidationError "Could not find reviewer user")
                    |> AsyncResult.map Some
                | None -> asyncResult { return None }

            let! updated =
                TodoPolicyService.buildUpdatedEntity
                    deps.PolicyDeps
                    actor
                    (command.Title, command.Description, command.DueDate, assignee, reviewer)
                    entity

            do! deps.Repository.Save(actor, updated) |> Async.Ignore
        }
