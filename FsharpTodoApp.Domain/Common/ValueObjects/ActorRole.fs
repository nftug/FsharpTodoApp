namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Enums

type ActorRole =
    | User
    | Manager
    | Admin

module ActorRole =
    let fromEnum =
        function
        | ActorRoleEnum.User -> User
        | ActorRoleEnum.Manager -> Manager
        | ActorRoleEnum.Admin -> Admin
        | _ -> User

    let toEnum =
        function
        | User -> ActorRoleEnum.User
        | Manager -> ActorRoleEnum.Manager
        | Admin -> ActorRoleEnum.Admin

    let isAtLeast role actorRole =
        match role |> toEnum, actorRole |> toEnum with
        | role, actorRole when role <= actorRole -> true
        | _ -> false
