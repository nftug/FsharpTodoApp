namespace FsharpTodoApp.Domain.Common.ValueObjects

type Actor =
    { UserInfo: UserInfo
      UserDbId: int64
      Roles: ActorRole list }

module Actor =
    let isAtLeast role actor =
        actor.Roles |> List.exists (ActorRole.isAtLeast role)

    let isUser userInfo actor = userInfo = actor.UserInfo

    let systemActor =
        { UserInfo = { UserName = "system" }
          UserDbId = 0L
          Roles = [ Admin ] }
