namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoAssignee = private TodoAssignee of UserInfo option

module TodoAssignee =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoAssignee assignee) = assignee

    let (|AssignAllowed|AssignBlocked|) (actor, assignee) =
        match value assignee with
        | None -> AssignAllowed
        | Some assignee when actor |> Actor.isAtLeast Manager || actor.UserInfo = assignee -> AssignAllowed
        | _ -> AssignBlocked

    let tryAssign ctx newAssignee =
        match ctx.Actor, TodoAssignee newAssignee with
        | AssignAllowed -> Ok(TodoAssignee newAssignee)
        | AssignBlocked -> Validation.error "Only admin can assign other users."
