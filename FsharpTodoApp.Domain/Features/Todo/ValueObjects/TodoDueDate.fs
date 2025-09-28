namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDueDate = private TodoDueDate of System.DateTime option

module TodoDueDate =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.ValueObjects

    let tryCreate (ctx: AuditContext<'a>) date =
        match date with
        | Some d when d < ctx.DateTimeProvider.UtcNow -> Validation.error "Due date cannot be in the past."
        | _ -> Ok(TodoDueDate date)

    let recreate date = TodoDueDate date

    let value (TodoDueDate current) = current
