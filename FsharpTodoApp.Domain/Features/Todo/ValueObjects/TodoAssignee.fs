namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoAssignee = private TodoAssignee of UserInfo option

module TodoAssignee =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoAssignee assignee) = assignee

    let (|AssigneeAllowed|AssigneeBlocked|NoAssignee|) (actor, current, next) =
        match actor, current, value next with
        | _, None, None -> NoAssignee
        | a, Some(TodoAssignee _), None when a |> Actor.isAtLeast Manager -> NoAssignee
        | a, _, Some next when a |> Actor.isAtLeast Manager || a |> Actor.isUser next -> AssigneeAllowed
        | _ -> AssigneeBlocked

    let (|CanActAssignee|CannotActAssignee|NoAssignee|) (actor, current) =
        match actor, value current with
        | _, None -> NoAssignee
        | a, Some cur when a |> Actor.isAtLeast Manager || a |> Actor.isUser cur -> CanActAssignee
        | _ -> CannotActAssignee

    let tryAssign ctx newAssignee =
        match ctx.Actor, None, TodoAssignee newAssignee with
        | AssigneeBlocked -> Validation.error "Only manager and admin can assign other users."
        | _ -> Ok(TodoAssignee newAssignee)

    let tryReassign ctx current newAssignee =
        match ctx.Actor, Some current, TodoAssignee newAssignee with
        | AssigneeBlocked -> Validation.error "Only manager and admin can reassign/unassign other users."
        | _ -> Ok(TodoAssignee newAssignee)

    let hydrate assigneeName =
        TodoAssignee(assigneeName |> Option.map (fun x -> { UserInfo.UserName = x }))
