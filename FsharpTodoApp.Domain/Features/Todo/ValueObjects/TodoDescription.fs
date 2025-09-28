namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDescription = private TodoDescription of string option

module TodoDescription =
    let maxLen = 500

    let tryCreate (text: string option) =
        match text with
        | Some t when t.Length > maxLen -> Error(sprintf "Description cannot be longer than %d characters." maxLen)
        | Some v -> Ok(TodoDescription(Some v))
        | None -> Ok(TodoDescription None)

    let recreate (text: string option) = TodoDescription text

    let value (TodoDescription text) = text
