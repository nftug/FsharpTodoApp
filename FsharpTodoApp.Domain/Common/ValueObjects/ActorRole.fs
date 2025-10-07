namespace FsharpTodoApp.Domain.Common.ValueObjects

type ActorRole =
    | User
    | Manager
    | Admin

module ActorRole =
    open FsharpTodoApp.Domain.Common.Enums

    let isAtLeast (role: ActorRole) (actorRole: ActorRole) =
        let toInt =
            function
            | User -> 0
            | Manager -> 1
            | Admin -> 2

        toInt actorRole >= toInt role

    let fromEnum (role: ActorRoleEnum) =
        match role with
        | ActorRoleEnum.User -> User
        | ActorRoleEnum.Manager -> Manager
        | ActorRoleEnum.Admin -> Admin
        | unknown -> invalidArg "role" (sprintf "Unknown ActorRoleEnum value: %A" unknown)

    let toEnum (role: ActorRole) =
        match role with
        | User -> ActorRoleEnum.User
        | Manager -> ActorRoleEnum.Manager
        | Admin -> ActorRoleEnum.Admin
