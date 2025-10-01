namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects

module PassThroughActorPolicy =
    let create actor =
        { Actor = actor
          CanCreate = true
          CanUpdate = true
          CanDelete = true }
