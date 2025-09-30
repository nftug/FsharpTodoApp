namespace FsharpTodoApp.Domain.Features.Todo.Policies

open FsharpTodoApp.Domain.Common.Policies
open FsharpTodoApp.Domain.Features.Auth.Policies
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.ValueObjects.TodoReviewer
open FsharpTodoApp.Domain.Features.Todo.ValueObjects.TodoAssignee

type TodoCreationPolicy(actor) =
    inherit PassThroughActorPolicy(actor)

type TodoUpdatePolicy(actor, entity) =
    inherit OwnerOnlyActorPolicy(actor, entity.Base)

type TodoDeletionPolicy(actor, entity) =
    inherit OwnerOnlyActorPolicy(actor, entity.Base)

type TodoUpdateStatusPolicy(actor, entity, newStatus) =
    interface IActorPolicy with
        member _.Actor = actor

        member _.CanCreate = false

        member _.CanUpdate =
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

        member _.CanDelete = false
