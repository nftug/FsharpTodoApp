namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Services
open FsharpTodoApp.Domain.Common.ValueObjects

type AuditContext =
    { Permission: Permission
      Actor: Actor
      DateTime: DateTimeProvider }

module AuditContext =
    let create (dateTimeProvider: DateTimeProvider) (actor: Actor) (permission: Permission) : AuditContext =
        { Permission = permission
          Actor = actor
          DateTime = dateTimeProvider }
