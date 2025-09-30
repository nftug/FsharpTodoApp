namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Services
open FsharpTodoApp.Domain.Common.Policies

type AuditContext<'T when 'T :> IActorPolicy> =
    { Policy: 'T
      Actor: Actor
      DateTime: IDateTimeProvider }

module AuditContext =
    let create dateTimeProvider actorPolicy =
        { Policy = actorPolicy
          Actor = actorPolicy.Actor
          DateTime = dateTimeProvider }
