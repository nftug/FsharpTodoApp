namespace FsharpTodoApp.Domain.Features.Auth.Policies

open FsharpTodoApp.Domain.Common.ValueObjects

type OwnerOnlyActorPolicy(actor: Actor, createdAudit: CreatedAudit option) =
    new(actor: Actor) = OwnerOnlyActorPolicy(actor, None)

    interface IActorPolicy with
        member _.Actor = actor

        member _.CanCreate = true

        member _.CanUpdate =
            actor.IsAdmin
            || createdAudit
               |> Option.map (fun a -> actor.UserInfo = (CreatedAudit.value a).UserInfo)
               |> Option.defaultValue false

        member _.CanDelete =
            actor.IsAdmin
            || createdAudit
               |> Option.map (fun a -> actor.UserInfo = (CreatedAudit.value a).UserInfo)
               |> Option.defaultValue false
