namespace FsharpTodoApp.Application.Features.Todo.Commands

module UpdateTodo =
    open FsharpTodoApp.Domain.Common.Utils
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsharpTodoApp.Domain.Features.User.Interfaces
    open FsharpTodoApp.Application.Features.Todo.Dtos.Commands
    open FsToolkit.ErrorHandling

    type Dependencies =
        { Repository: ITodoRepository
          UserRef: IUserReferenceService
          PolicyDeps: TodoPolicyService.Dependencies }

    let handle deps (actor, id, command: TodoUpdateCommandDto) =
        taskResult {
            let! entity = deps.Repository.GetById(Some actor, id) |> TaskResult.requireSome NotFoundError

            let! assignee =
                deps.UserRef.GetByUserName
                |> TaskResult.maybeFetch (ValidationError "Could not find assignee user")
                <| command.AssigneeUserName

            let! reviewer =
                deps.UserRef.GetByUserName
                |> TaskResult.maybeFetch (ValidationError "Could not find reviewer user")
                <| command.ReviewerUserName

            let! updated =
                TodoPolicyService.buildUpdated
                    deps.PolicyDeps
                    actor
                    (command.Title, command.Description, command.DueDate, assignee, reviewer)
                    entity

            do! deps.Repository.Save(actor, updated) |> Task.ignore
        }
