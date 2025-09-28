namespace FsharpTodoApp.Domain.Common.Services

type IDateTimeProvider =
    abstract member UtcNow: System.DateTime

type DateTimeProvider() =
    interface IDateTimeProvider with
        member _.UtcNow = System.DateTime.UtcNow
