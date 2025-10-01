namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Entities

module OwnerOnlyActorPolicy =
    let create actor entityBase =
        { Actor = actor
          CanCreate = true
          CanUpdate =
            actor |> Actor.isAtLeast Admin
            || actor |> Actor.isUser (CreatedAudit.value entityBase.CreatedAudit).UserInfo
          CanDelete =
            actor |> Actor.isAtLeast Admin
            || actor |> Actor.isUser (CreatedAudit.value entityBase.CreatedAudit).UserInfo }
