namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Features.Todo.Enums

type TodoStatus = private TodoStatus of TodoStatusEnum

module TodoStatus =
    open FsharpTodoApp.Domain.Common.Errors

    let start = TodoStatus TodoStatusEnum.Todo

    let recreate status = TodoStatus status

    let tryUpdate newStatus (TodoStatus current) =
        match current, newStatus with
        | c, n when c = n -> Ok(TodoStatus n)
        | TodoStatusEnum.Todo, TodoStatusEnum.InProgress -> Ok(TodoStatus newStatus)
        | TodoStatusEnum.InProgress, TodoStatusEnum.Done -> Ok(TodoStatus newStatus)
        | _ -> Error(ValidationError "Invalid status transition.")

    let value (TodoStatus current) = current
