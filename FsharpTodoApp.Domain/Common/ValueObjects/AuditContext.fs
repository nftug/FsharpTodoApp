namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Services
open FsharpTodoApp.Domain.Common.Policies

type AuditContext<'T when 'T :> IActorPolicy> =
    { Policy: 'T
      DateTimeProvider: IDateTimeProvider }
