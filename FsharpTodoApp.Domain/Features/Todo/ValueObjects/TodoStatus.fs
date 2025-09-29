namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Features.Todo.Enums

type TodoStatusValue =
    | Todo
    | InProgress
    | Done

type TodoStatus = private TodoStatus of TodoStatusValue

module TodoStatus =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.ValueObjects

    let fromEnum =
        function
        | TodoStatusEnum.Todo -> Todo
        | TodoStatusEnum.InProgress -> InProgress
        | TodoStatusEnum.Done -> Done
        | unknown -> invalidArg "status" (sprintf "Unknown TodoStatusEnum value: %A" unknown)

    let toEnum =
        function
        | Todo -> TodoStatusEnum.Todo
        | InProgress -> TodoStatusEnum.InProgress
        | Done -> TodoStatusEnum.Done

    let getNeighbor =
        function
        | Todo -> None, Some InProgress
        | InProgress -> Some Todo, Some Done
        | Done -> Some InProgress, None

    let defaultStatus = TodoStatus Todo

    let tryUpdate ctx newStatus =
        match ctx.Policy.CanUpdate with
        | true -> Ok(TodoStatus newStatus)
        | false -> Validation.error "Cannot update status due to policy."

    let value (TodoStatus status) = status

    let recreate status = status |> fromEnum |> TodoStatus
