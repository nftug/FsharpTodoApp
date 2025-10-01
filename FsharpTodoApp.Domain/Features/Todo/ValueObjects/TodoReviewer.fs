namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoReviewer = private TodoReviewer of UserInfo option

module TodoReviewer =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoReviewer reviewer) = reviewer

    let (|ReviewerAllowed|ReviewerBlocked|NoReviewer|) (actor, current, next) =
        match actor, current, value next with
        | _, None, None -> NoReviewer
        | a, Some(TodoReviewer _), None when a |> Actor.isAtLeast Manager -> NoReviewer
        | a, _, Some next when a |> Actor.isAtLeast Manager || a |> Actor.isUser next -> ReviewerAllowed
        | _ -> ReviewerBlocked

    let (|CanActReviewer|CannotActReviewer|NoReviewer|) (actor, current) =
        match actor, value current with
        | _, None -> NoReviewer
        | a, Some cur when a |> Actor.isAtLeast Manager || a |> Actor.isUser cur -> CanActReviewer
        | _ -> CannotActReviewer

    let tryAssign ctx newReviewer =
        match ctx.Actor, None, TodoReviewer newReviewer with
        | ReviewerBlocked -> Validation.error "Only manager and admin can assign other users."
        | _ -> Ok(TodoReviewer newReviewer)

    let tryReassign ctx current newReviewer =
        match ctx.Actor, Some current, TodoReviewer newReviewer with
        | ReviewerBlocked -> Validation.error "Only manager and admin can reassign/unassign other users."
        | _ -> Ok(TodoReviewer newReviewer)

    let hydrate reviewerName =
        TodoReviewer(reviewerName |> Option.map (fun x -> { UserInfo.UserName = x }))
