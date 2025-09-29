namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Features.Todo.Enums

type TodoStatusValue =
    | Todo
    | InProgress
    | Done

type TodoStatus = private TodoStatus of TodoStatusValue

module TodoStatus =
    open FsharpTodoApp.Domain.Common.Errors

    let private fromEnum =
        function
        | TodoStatusEnum.Todo -> Todo
        | TodoStatusEnum.InProgress -> InProgress
        | TodoStatusEnum.Done -> Done
        | unknown -> invalidArg "status" (sprintf "Unknown TodoStatusEnum value: %A" unknown)

    let private toEnum =
        function
        | Todo -> TodoStatusEnum.Todo
        | InProgress -> TodoStatusEnum.InProgress
        | Done -> TodoStatusEnum.Done

    let getNeighbor =
        function
        | Todo -> None, Some InProgress
        | InProgress -> Some Todo, Some Done
        | Done -> Some InProgress, None

    let start = TodoStatus Todo

    let tryUpdate ctx reviewer newStatusEnum (TodoStatus currentStatus) =
        let newStatus = newStatusEnum |> fromEnum

        match currentStatus, newStatus, (ctx, reviewer) with
        | c, n, TodoReviewer.CannotReview when c = Done || n = Done ->
            Validation.error "Only admin or the reviewer can change the status to/from Done."
        | _, n, _ -> Ok(TodoStatus n)

    let value (TodoStatus status) = status

    let enumValue (TodoStatus status) = status |> toEnum

    let recreate status = status |> fromEnum |> TodoStatus
