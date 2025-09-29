namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDueDate = private TodoDueDate of System.DateTime option

module TodoDueDate =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.ValueObjects

    let tryCreate ctx =
        function
        | Some d when d < ctx.DateTimeProvider.UtcNow -> Validation.error "Due date cannot be in the past."
        | d -> Ok(TodoDueDate d)

    let recreate date = TodoDueDate date

    let value (TodoDueDate current) = current
