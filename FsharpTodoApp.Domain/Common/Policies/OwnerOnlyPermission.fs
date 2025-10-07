namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Entities

module OwnerOnlyPermission =
    let create (actor: Actor) (entityBase: EntityBase) : Permission =
        { CanCreate = true
          CanUpdate =
            actor |> Actor.isAtLeast Admin
            || actor |> Actor.isUser (CreatedAudit.value entityBase.CreatedAudit).UserInfo
          CanDelete =
            actor |> Actor.isAtLeast Admin
            || actor |> Actor.isUser (CreatedAudit.value entityBase.CreatedAudit).UserInfo }
