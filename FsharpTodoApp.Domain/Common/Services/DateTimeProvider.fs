namespace FsharpTodoApp.Domain.Common.Services

type DateTimeProvider = { UtcNow: unit -> System.DateTime }

module DateTimeProvider =
    let create () : DateTimeProvider =
        { UtcNow = fun () -> System.DateTime.UtcNow }
