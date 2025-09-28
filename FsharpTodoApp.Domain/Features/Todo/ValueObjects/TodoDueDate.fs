namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDueDate = private TodoDueDate of System.DateTime option

module TodoDueDate =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.Services

    let tryCreate (dateTimeProvider: IDateTimeProvider) date =
        match date with
        | Some d when d < dateTimeProvider.UtcNow -> Validation.error "Due date cannot be in the past."
        | _ -> Ok(TodoDueDate date)

    let recreate date = TodoDueDate date

    let value (TodoDueDate current) = current
