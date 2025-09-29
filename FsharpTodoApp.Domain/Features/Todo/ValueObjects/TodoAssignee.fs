namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoAssignee = private TodoAssignee of UserInfo option

module TodoAssignee =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoAssignee assignee) = assignee

    let (|AssignAllowed|AssignBlocked|) (actor, assignee) =
        match actor, value assignee with
        | _, None -> AssignAllowed
        | a, _ when a |> Actor.isAtLeast Manager -> AssignAllowed
        | a, Some assigneeInfo when a.UserInfo = assigneeInfo -> AssignAllowed
        | _ -> AssignBlocked

    let tryAssign ctx newAssignee =
        match ctx.Actor, TodoAssignee newAssignee with
        | AssignAllowed -> Ok(TodoAssignee newAssignee)
        | AssignBlocked -> Validation.error "Only manager and admin can assign other users."

    let recreate assignee = TodoAssignee assignee
