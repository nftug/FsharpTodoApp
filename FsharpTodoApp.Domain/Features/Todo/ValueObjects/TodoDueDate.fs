namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDueDate = private TodoDueDate of System.DateTime option

module TodoDueDate =
    open FsharpTodoApp.Domain.Common.Errors
    open FsharpTodoApp.Domain.Common.ValueObjects
    open FsharpTodoApp.Domain.Common.Services

    let tryCreate (ctx: AuditContext) (datetime: System.DateTime option) : Result<TodoDueDate, AppError> =
        match datetime with
        | Some dt when dt < ctx.DateTime.UtcNow() -> Validation.error "Due date cannot be in the past."
        | dt -> Ok(TodoDueDate dt)

    let hydrate (date: System.DateTime option) : TodoDueDate = TodoDueDate date

    let value (TodoDueDate current: TodoDueDate) : System.DateTime option = current
