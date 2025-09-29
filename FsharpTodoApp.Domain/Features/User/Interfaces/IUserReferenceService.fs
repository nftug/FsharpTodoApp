namespace FsharpTodoApp.Domain.Features.User.Interfaces

open FsharpTodoApp.Domain.Common.ValueObjects

type IUserReferenceService =
    abstract member GetByUserName: string -> Async<UserInfo option>
