namespace FsharpTodoApp.Domain.Features.Auth.Policies

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Policies
open FsharpTodoApp.Domain.Common.Entities

type OwnerOnlyActorPolicy(actor: Actor, entityBase: EntityBase option) =
    new(actor: Actor) = OwnerOnlyActorPolicy(actor, None)

    interface IActorPolicy with
        member _.Actor = actor

        member _.CanCreate = true

        member _.CanUpdate =
            actor.IsAdmin
            || entityBase
               |> Option.map (fun e -> actor.UserInfo = e.CreatedAudit.UserInfo)
               |> Option.defaultValue false

        member _.CanDelete =
            actor.IsAdmin
            || entityBase
               |> Option.map (fun e -> actor.UserInfo = e.CreatedAudit.UserInfo)
               |> Option.defaultValue false
