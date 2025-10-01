namespace FsharpTodoApp.Application.Features.Todo.Enums

open System.Text.Json.Serialization
open FsharpTodoApp.Domain.Features.Todo.ValueObjects

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type TodoStatusEnum =
    | Todo = 0
    | InProgress = 1
    | Done = 2

module TodoStatusEnum =
    let fromDomain =
        function
        | Todo -> TodoStatusEnum.Todo
        | InProgress -> TodoStatusEnum.InProgress
        | Done -> TodoStatusEnum.Done

    let ofDomain =
        function
        | TodoStatusEnum.Todo -> Todo
        | TodoStatusEnum.InProgress -> InProgress
        | TodoStatusEnum.Done -> Done
        | unknown -> invalidArg "status" (sprintf "Unknown TodoStatusEnum value: %A" unknown)
