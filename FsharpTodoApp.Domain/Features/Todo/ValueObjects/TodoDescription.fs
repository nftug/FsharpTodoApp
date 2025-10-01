namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDescription = private TodoDescription of string option

module TodoDescription =
    open FsharpTodoApp.Domain.Common.Errors

    let maxLen = 500

    let tryCreate (text: string option) =
        match text with
        | Some s when s.Length > maxLen -> Validation.errorf "Description cannot be longer than %d characters." maxLen
        | v -> Ok(TodoDescription v)

    let hydrate text = TodoDescription text

    let value (TodoDescription current) = current
