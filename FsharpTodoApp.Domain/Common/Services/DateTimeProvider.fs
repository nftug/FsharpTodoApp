namespace FsharpTodoApp.Domain.Common.Services

type DateTimeProvider = { UtcNow: unit -> System.DateTime }

module DateTimeProvider =
    let create () =
        { UtcNow = fun () -> System.DateTime.UtcNow }
