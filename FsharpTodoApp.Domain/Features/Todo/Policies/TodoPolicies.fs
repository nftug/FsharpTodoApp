namespace FsharpTodoApp.Domain.Features.Todo.Policies

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Policies
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.ValueObjects.TodoReviewer
open FsharpTodoApp.Domain.Features.Todo.ValueObjects.TodoAssignee

module TodoCreationPolicy =
    let create actor = PassThroughActorPolicy.create actor

module TodoUpdatePolicy =
    let create actor entity =
        OwnerOnlyActorPolicy.create actor entity.Base

module TodoDeletionPolicy =
    let create actor entity =
        OwnerOnlyActorPolicy.create actor entity.Base

module TodoUpdateStatusPolicy =
    let create actor entity newStatus =
        { Actor = actor
          CanCreate = false
          CanUpdate =
            let reviewer = entity.Reviewer
            let assignee = entity.Assignee
            let status = entity.Status |> TodoStatus.value

            match status, newStatus, (actor, reviewer), (actor, assignee) with
            // Done status can be changed only by admin or the reviewer
            | c, n, CannotActReviewer, _ when c = Done || n = Done -> false
            // If actor is neither the reviewer nor the assignee, cannot change status
            | _, _, CannotActReviewer, CannotActAssignee -> false
            // In all other cases, allow status change
            | _ -> true
          CanDelete = false }
