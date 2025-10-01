namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Services
open FsharpTodoApp.Domain.Common.ValueObjects

type AuditContext =
    { Policy: ActorPolicy
      Actor: Actor
      DateTime: IDateTimeProvider }

module AuditContext =
    let create dateTimeProvider actorPolicy =
        { Policy = actorPolicy
          Actor = actorPolicy.Actor
          DateTime = dateTimeProvider }
