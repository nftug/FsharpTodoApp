namespace FsharpTodoApp.Domain.Features.Auth.Policies

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Policies
open FsharpTodoApp.Domain.Common.Entities

type OwnerOnlyActorPolicy(actor: Actor, entityBase: EntityBase) =
    interface IActorPolicy with
        member _.Actor = actor

        member _.CanCreate = true

        member _.CanUpdate =
            actor |> Actor.isAtLeast Admin
            || actor |> Actor.isUser entityBase.CreatedAudit.UserInfo

        member _.CanDelete =
            actor |> Actor.isAtLeast Admin
            || actor |> Actor.isUser entityBase.CreatedAudit.UserInfo
