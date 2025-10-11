namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Errors

type TodoAssignee = private TodoAssignee of UserInfo option

module TodoAssignee =
    let value (TodoAssignee assignee: TodoAssignee) : UserInfo option = assignee

    let (|AssigneeAllowed|AssigneeBlocked|NoAssignee|) (actor, current, next) =
        match actor, value current, value next with
        | _, None, None -> AssigneeAllowed
        | a, Some _, None when a |> Actor.isAtLeast Manager -> NoAssignee
        | a, _, Some next when a |> Actor.isAtLeast Manager || a |> Actor.isUser next -> AssigneeAllowed
        | _ -> AssigneeBlocked

    let (|CanActAssignee|CannotActAssignee|NoAssignee|) (actor, current) =
        match actor, value current with
        | _, None -> NoAssignee
        | a, Some cur when a |> Actor.isAtLeast Manager || a |> Actor.isUser cur -> CanActAssignee
        | _ -> CannotActAssignee

    let tryAssign (ctx: AuditContext) (newAssignee: UserInfo option) : Result<TodoAssignee, AppError> =
        match ctx.Actor, TodoAssignee None, TodoAssignee newAssignee with
        | AssigneeBlocked -> Validation.error "Only manager and admin can assign other assignees."
        | _ -> Ok(TodoAssignee newAssignee)

    let tryReassign
        (ctx: AuditContext)
        (current: TodoAssignee)
        (newAssignee: UserInfo option)
        : Result<TodoAssignee, AppError> =
        match ctx.Actor, current, TodoAssignee newAssignee with
        | AssigneeBlocked -> Validation.error "Only manager and admin can reassign/unassign other assignees."
        | _ -> Ok(TodoAssignee newAssignee)

    let hydrate (assigneeName: string option) : TodoAssignee =
        TodoAssignee(assigneeName |> Option.map (fun x -> { UserInfo.UserName = x }))
