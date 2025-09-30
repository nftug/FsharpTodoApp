namespace FsharpTodoApp.Domain.Common.ValueObjects

type Actor =
    { UserInfo: UserInfo
      UserDbId: int64
      Role: ActorRole }

module Actor =
    let isAtLeast role actor = actor.Role |> ActorRole.isAtLeast role

    let isUser userInfo actor = userInfo = actor.UserInfo
