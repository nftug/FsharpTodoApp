namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDueDate = private TodoDueDate of System.DateTime option

module TodoDueDate =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.ValueObjects

    let tryCreate ctx datetime =
        match datetime with
        | Some dt when dt < ctx.DateTime.UtcNow -> Validation.error "Due date cannot be in the past."
        | dt -> Ok(TodoDueDate dt)

    let hydrate date = TodoDueDate date

    let value (TodoDueDate current) = current
