namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoReviewer = private TodoReviewer of UserInfo option

module TodoReviewer =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoReviewer reviewer) = reviewer

    let (|ReviewAllowed|ReviewBlocked|) (actor, reviewer) =
        match actor, value reviewer with
        | _, None -> ReviewAllowed
        | a, _ when a |> Actor.isAtLeast Manager -> ReviewAllowed
        | a, Some reviewerInfo when a.UserInfo = reviewerInfo -> ReviewAllowed
        | _ -> ReviewBlocked

    let tryAssign ctx newReviewer =
        match ctx.Actor, TodoReviewer newReviewer with
        | ReviewAllowed -> Ok(TodoReviewer newReviewer)
        | ReviewBlocked -> Validation.error "Only manager and admin can assign other users."

    let recreate reviewer = TodoReviewer reviewer
