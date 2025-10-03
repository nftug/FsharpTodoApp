namespace FsharpTodoApp.Domain.Common.Enums

open System.Text.Json.Serialization
open FsharpTodoApp.Domain.Common.ValueObjects

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type ActorRoleEnum =
    | User = 0
    | Manager = 1
    | Admin = 2

module ActorRoleEnum =
    let fromDomain =
        function
        | User -> ActorRoleEnum.User
        | Manager -> ActorRoleEnum.Manager
        | Admin -> ActorRoleEnum.Admin

    let ofDomain =
        function
        | ActorRoleEnum.User -> User
        | ActorRoleEnum.Manager -> Manager
        | ActorRoleEnum.Admin -> Admin
        | unknown -> invalidArg "role" (sprintf "Unknown ActorRoleEnum value: %A" unknown)
