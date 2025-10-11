namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Errors

type TodoReviewer = private TodoReviewer of UserInfo option

module TodoReviewer =
    let value (TodoReviewer reviewer) = reviewer

    let (|ReviewerAllowed|ReviewerBlocked|NoReviewer|) (actor, current, next) =
        match actor, value current, value next with
        | _, None, None -> NoReviewer
        | a, Some _, None when a |> Actor.isAtLeast Manager -> NoReviewer
        | a, _, Some next when a |> Actor.isAtLeast Manager || a |> Actor.isUser next -> ReviewerAllowed
        | _ -> ReviewerBlocked

    let (|CanActReviewer|CannotActReviewer|NoReviewer|) (actor, current) =
        match actor, value current with
        | _, None -> NoReviewer
        | a, Some cur when a |> Actor.isAtLeast Manager || a |> Actor.isUser cur -> CanActReviewer
        | _ -> CannotActReviewer

    let tryAssign (ctx: AuditContext) (newReviewer: UserInfo option) : Result<TodoReviewer, AppError> =
        match ctx.Actor, TodoReviewer None, TodoReviewer newReviewer with
        | ReviewerBlocked -> Validation.error "Only manager and admin can assign other reviewers."
        | _ -> Ok(TodoReviewer newReviewer)

    let tryReassign
        (ctx: AuditContext)
        (current: TodoReviewer)
        (newReviewer: UserInfo option)
        : Result<TodoReviewer, AppError> =
        match ctx.Actor, current, TodoReviewer newReviewer with
        | ReviewerBlocked -> Validation.error "Only manager and admin can reassign/unassign other reviewers."
        | _ -> Ok(TodoReviewer newReviewer)

    let hydrate (reviewerName: string option) : TodoReviewer =
        TodoReviewer(reviewerName |> Option.map (fun x -> { UserInfo.UserName = x }))
