namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects

type PassThroughActorPolicy(actor: Actor) =
    interface IActorPolicy with
        member _.Actor = actor

        member _.CanCreate = true

        member _.CanUpdate = true

        member _.CanDelete = true
