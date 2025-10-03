namespace FsharpTodoApp.Domain.Common.ValueObjects

type ActorRole =
    | User
    | Manager
    | Admin

module ActorRole =
    open FsharpTodoApp.Domain.Common.Enums

    let isAtLeast role actorRole =
        let toInt =
            function
            | User -> 0
            | Manager -> 1
            | Admin -> 2

        toInt actorRole >= toInt role

    let fromEnum =
        function
        | ActorRoleEnum.User -> User
        | ActorRoleEnum.Manager -> Manager
        | ActorRoleEnum.Admin -> Admin
        | unknown -> invalidArg "role" (sprintf "Unknown ActorRoleEnum value: %A" unknown)

    let toEnum =
        function
        | User -> ActorRoleEnum.User
        | Manager -> ActorRoleEnum.Manager
        | Admin -> ActorRoleEnum.Admin
