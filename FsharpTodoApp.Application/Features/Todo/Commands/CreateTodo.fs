namespace FsharpTodoApp.Application.Features.Todo.Commands

module CreateTodo =
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

    let handle deps (actor, command: TodoCreateCommandDto) =
        taskResult {
            let! assignee =
                match command.AssigneeUserName with
                | Some arg ->
                    deps.UserRef.GetByUserName arg
                    |> TaskResult.requireSome (ValidationError "Could not find assignee user")
                    |> TaskResult.map Some
                | None -> taskResult { return None }

            let! reviewer =
                match command.ReviewerUserName with
                | Some arg ->
                    deps.UserRef.GetByUserName arg
                    |> TaskResult.requireSome (ValidationError "Could not find reviewer user")
                    |> TaskResult.map Some
                | None -> taskResult { return None }

            let! entity =
                TodoPolicyService.buildCreated
                    deps.PolicyDeps
                    actor
                    (command.Title, command.Description, command.DueDate, assignee, reviewer)

            return
                deps.Repository.Save(actor, entity)
                |> Task.map (fun x -> x.Base |> ItemCreatedResponseDto.create)
        }
