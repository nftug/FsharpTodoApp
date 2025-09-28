namespace FsharpTodoApp.Domain.Common.ValueObjects

type Actor =
    { UserInfo: UserInfo
      UserDbId: int64
      IsAdmin: bool }
