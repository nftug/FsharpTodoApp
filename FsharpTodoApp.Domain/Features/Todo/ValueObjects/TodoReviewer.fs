namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoReviewer = private TodoReviewer of UserInfo option

module TodoReviewer =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoReviewer reviewer) = reviewer

    let (|ReviewAllowed|ReviewBlocked|) (actor, reviewer) =
        match value reviewer with
        | None -> ReviewAllowed
        | Some reviewerInfo when actor |> Actor.isAtLeast Manager || actor.UserInfo = reviewerInfo -> ReviewAllowed
        | _ -> ReviewBlocked

    let tryAssign ctx newReviewer =
        match ctx.Actor, TodoReviewer newReviewer with
        | ReviewAllowed -> Ok(TodoReviewer newReviewer)
        | ReviewBlocked -> Validation.error "Only admin can assign other users."

    let recreate reviewer = TodoReviewer reviewer
