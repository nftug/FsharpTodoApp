namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoDueDate = private TodoDueDate of System.DateTime option

module TodoDueDate =
    let tryCreate (date: System.DateTime option) =
        // 期限日は過去日を許容しない
        match date with
        | Some d when d < System.DateTime.UtcNow.Date -> Error "Due date cannot be in the past."
        | _ -> Ok(TodoDueDate date)

    let recreate (date: System.DateTime option) = TodoDueDate date

    let value (TodoDueDate date) = date
