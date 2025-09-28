namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Services

type EntityAudit =
    { User: UserInfo
      Timestamp: System.DateTime }

module EntityAudit =
    let create (actor: Actor) (dateTimeProvider: IDateTimeProvider) =
        { User = actor.UserInfo
          Timestamp = dateTimeProvider.UtcNow }
