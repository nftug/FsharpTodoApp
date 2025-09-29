namespace FsharpTodoApp.Domain.Common.ValueObjects

type ActorRole =
    | User
    | Manager
    | Admin

module ActorRole =
    let isAtLeast role actorRole =
        let toInt =
            function
            | User -> 0
            | Manager -> 1
            | Admin -> 2

        toInt actorRole >= toInt role
