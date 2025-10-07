namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects

module AdminOnlyPermission =
    let create (actor: Actor) : Permission =
        match actor with
        | a when a.Roles |> List.contains Admin ->
            { CanCreate = true
              CanUpdate = true
              CanDelete = true }
        | _ ->
            { CanCreate = false
              CanUpdate = false
              CanDelete = false }
