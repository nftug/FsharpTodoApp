namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoStatusValue =
    | Todo
    | InProgress
    | Done

type TodoStatus = private TodoStatus of TodoStatusValue

module TodoStatus =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.ValueObjects
    open FsharpTodoApp.Domain.Features.Todo.Enums

    let getNeighbor (status: TodoStatusValue) : (TodoStatusValue option * TodoStatusValue option) =
        match status with
        | Todo -> None, Some InProgress
        | InProgress -> Some Todo, Some Done
        | Done -> Some InProgress, None

    let defaultStatus: TodoStatus = TodoStatus Todo

    let tryUpdate (ctx: AuditContext) (newStatus: TodoStatusValue) : Result<TodoStatus, AppError> =
        match ctx.Permission.CanUpdate with
        | true -> Ok(TodoStatus newStatus)
        | false -> Validation.error "Cannot update status due to permission."

    let value (TodoStatus status: TodoStatus) : TodoStatusValue = status

    let hydrate (status: TodoStatusValue) : TodoStatus = status |> TodoStatus

    let fromEnum (status: TodoStatusEnum) : TodoStatusValue =
        match status with
        | TodoStatusEnum.Todo -> Todo
        | TodoStatusEnum.InProgress -> InProgress
        | TodoStatusEnum.Done -> Done
        | unknown -> invalidArg "status" (sprintf "Unknown TodoStatusEnum value: %A" unknown)

    let toEnum (status: TodoStatusValue) : TodoStatusEnum =
        match status with
        | Todo -> TodoStatusEnum.Todo
        | InProgress -> TodoStatusEnum.InProgress
        | Done -> TodoStatusEnum.Done
