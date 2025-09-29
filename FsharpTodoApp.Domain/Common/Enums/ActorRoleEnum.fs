namespace FsharpTodoApp.Domain.Common.Enums

open System.Text.Json.Serialization

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type ActorRoleEnum =
    | User = 0
    | Manager = 1
    | Admin = 2
