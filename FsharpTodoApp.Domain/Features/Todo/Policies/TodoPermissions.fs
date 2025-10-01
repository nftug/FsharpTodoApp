namespace FsharpTodoApp.Domain.Features.Todo.Policies

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Policies
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.ValueObjects.TodoReviewer
open FsharpTodoApp.Domain.Features.Todo.ValueObjects.TodoAssignee

module TodoCreationPermission =
    let create = PassThroughPermission.create

module TodoUpdatePermission =
    let create actor entity =
        OwnerOnlyPermission.create actor entity.Base

module TodoDeletionPermission =
    let create actor entity =
        OwnerOnlyPermission.create actor entity.Base

module TodoUpdateStatusPermission =
    let create actor entity newStatus =
        { CanCreate = false
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
