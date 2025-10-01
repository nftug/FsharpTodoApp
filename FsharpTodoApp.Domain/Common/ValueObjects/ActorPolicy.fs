namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.ValueObjects

type ActorPolicy =
    { Actor: Actor
      CanCreate: bool
      CanUpdate: bool
      CanDelete: bool }
