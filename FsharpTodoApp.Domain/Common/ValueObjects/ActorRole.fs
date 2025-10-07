namespace FsharpTodoApp.Domain.Common.ValueObjects

type ActorRole =
    | User
    | Manager
    | Admin

module ActorRole =
    open FsharpTodoApp.Domain.Common.Enums

    let isAtLeast (actorRole: ActorRole) (roles: ActorRole list) : bool =
        let toInt =
            function
            | User -> 0
            | Manager -> 1
            | Admin -> 2

        roles |> List.exists (fun role -> toInt role >= toInt actorRole)

    let fromEnum (role: ActorRoleEnum) : ActorRole =
        match role with
        | ActorRoleEnum.User -> User
        | ActorRoleEnum.Manager -> Manager
        | ActorRoleEnum.Admin -> Admin
        | unknown -> invalidArg "role" (sprintf "Unknown ActorRoleEnum value: %A" unknown)

    let toEnum (role: ActorRole) : ActorRoleEnum =
        match role with
        | User -> ActorRoleEnum.User
        | Manager -> ActorRoleEnum.Manager
        | Admin -> ActorRoleEnum.Admin
