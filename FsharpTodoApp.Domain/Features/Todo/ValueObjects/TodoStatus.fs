namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoStatusValue =
    | Todo
    | InProgress
    | Done

type TodoStatus = private TodoStatus of TodoStatusValue

module TodoStatus =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.ValueObjects

    let getNeighbor =
        function
        | Todo -> None, Some InProgress
        | InProgress -> Some Todo, Some Done
        | Done -> Some InProgress, None

    let defaultStatus = TodoStatus Todo

    let tryUpdate ctx newStatus =
        match ctx.Permission.CanUpdate with
        | true -> Ok(TodoStatus newStatus)
        | false -> Validation.error "Cannot update status due to permission."

    let value (TodoStatus status) = status

    let hydrate status = status |> TodoStatus
