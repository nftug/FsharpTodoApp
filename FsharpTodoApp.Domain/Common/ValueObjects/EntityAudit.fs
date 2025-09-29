namespace FsharpTodoApp.Domain.Common.ValueObjects

type EntityAudit =
    { UserInfo: UserInfo
      Timestamp: System.DateTime }

module EntityAudit =
    let create ctx =
        { UserInfo = ctx.Actor.UserInfo
          Timestamp = ctx.DateTimeProvider.UtcNow }
