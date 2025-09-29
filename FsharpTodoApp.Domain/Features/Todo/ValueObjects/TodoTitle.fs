namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoTitle = private TodoTitle of string

module TodoTitle =
    open FsharpTodoApp.Domain.Common.Errors
    open System

    let maxLen = 100

    let tryCreate =
        function
        | t when String.IsNullOrWhiteSpace t -> Validation.error "Title cannot be empty."
        | t when t.Length > maxLen -> Validation.errorf "Title cannot be longer than %d characters." maxLen
        | t -> Ok(TodoTitle t)

    let recreate text = TodoTitle text

    let value (TodoTitle current) = current
