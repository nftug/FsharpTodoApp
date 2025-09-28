namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects

type IActorPolicy =
    abstract member Actor: Actor
    abstract member CanCreate: bool
    abstract member CanUpdate: bool
    abstract member CanDelete: bool
