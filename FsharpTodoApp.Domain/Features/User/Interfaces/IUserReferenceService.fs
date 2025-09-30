namespace FsharpTodoApp.Domain.Features.User.Interfaces

open FsharpTodoApp.Domain.Common.ValueObjects

type IUserReferenceService =
    abstract member GetByUserNameAsync: string -> Async<UserInfo option>
