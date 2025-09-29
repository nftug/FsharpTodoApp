namespace FsharpTodoApp.Domain.Features.Todo.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type TodoReviewer = private TodoReviewer of UserInfo option

module TodoReviewer =
    open FsharpTodoApp.Domain.Common.Errors

    let value (TodoReviewer reviewer) = reviewer

    let (|CanReview|CannotReview|) (ctx, reviewer) =
        match value reviewer with
        | None -> CanReview
        | Some reviewerInfo when ctx.Policy.Actor.IsAdmin || ctx.Policy.Actor.UserInfo = reviewerInfo -> CanReview
        | _ -> CannotReview

    let tryAssign ctx newReviewer =
        match ctx, TodoReviewer newReviewer with
        | CanReview -> Ok(TodoReviewer newReviewer)
        | CannotReview -> Validation.error "Only admin can assign other users."

    let recreate reviewer = TodoReviewer reviewer
