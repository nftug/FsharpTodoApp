namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoTitle = private TodoTitle of string

module TodoTitle =
    open FsharpTodoApp.Domain.Common.Errors

    let maxLen = 100

    let tryCreate text =
        match text with
        | t when System.String.IsNullOrWhiteSpace t -> Error(ValidationError "Title cannot be empty.")
        | t when t.Length > maxLen ->
            Error(ValidationError(sprintf "Title cannot be longer than %d characters." maxLen))
        | _ -> Ok(TodoTitle text)

    let recreate text = TodoTitle text

    let value (TodoTitle current) = current
