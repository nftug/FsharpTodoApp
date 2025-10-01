namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoReviewer = private TodoReviewer of UserInfo option

module TodoReviewer =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoReviewer reviewer) = reviewer

    let (|ReviewerAllowed|ReviewerBlocked|NoReviewer|) (actor, next, current) =
        match actor, value next, current with
        | _, None, None -> NoReviewer
        | a, None, Some(TodoReviewer _) when a |> Actor.isAtLeast Manager -> NoReviewer
        | a, Some next, _ when a |> Actor.isAtLeast Manager || a |> Actor.isUser next -> ReviewerAllowed
        | _ -> ReviewerBlocked

    let (|CanActReviewer|CannotActReviewer|NoReviewer|) (actor, current) =
        match actor, value current with
        | _, None -> NoReviewer
        | a, Some cur when a |> Actor.isAtLeast Manager || a |> Actor.isUser cur -> CanActReviewer
        | _ -> CannotActReviewer

    let tryAssign ctx newReviewer =
        match ctx.Actor, TodoReviewer newReviewer, None with
        | ReviewerBlocked -> Validation.error "Only manager and admin can assign other users."
        | _ -> Ok(TodoReviewer newReviewer)

    let tryReassign ctx current newReviewer =
        match ctx.Actor, TodoReviewer newReviewer, Some current with
        | ReviewerBlocked -> Validation.error "Only manager and admin can reassign/unassign other users."
        | _ -> Ok(TodoReviewer newReviewer)

    let hydrate reviewerName =
        TodoReviewer(reviewerName |> Option.map (fun x -> { UserInfo.UserName = x }))
