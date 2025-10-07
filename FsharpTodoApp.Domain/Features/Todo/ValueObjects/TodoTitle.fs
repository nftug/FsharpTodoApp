namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoTitle = private TodoTitle of string

module TodoTitle =
    open FsharpTodoApp.Domain.Common.Errors
    open System

    let maxLen = 100

    let tryCreate (title: string) : Result<TodoTitle, AppError> =
        match title with
        | t when String.IsNullOrWhiteSpace t -> Validation.error "Title cannot be empty."
        | t when t.Length > maxLen -> Validation.errorf "Title cannot be longer than %d characters." maxLen
        | t -> Ok(TodoTitle t)

    let hydrate (text: string) : TodoTitle = TodoTitle text

    let value (TodoTitle current: TodoTitle) = current
