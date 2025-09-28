namespace FsharpTodoApp.Domain.Features.Todo.Enums

open System.Text.Json.Serialization

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type TodoStatusEnum =
    | Todo = 0
    | InProgress = 1
    | Done = 2
