namespace FsharpTodoApp.Domain.Features.Todo.Services

module TodoPolicyService =
    open FsharpTodoApp.Domain.Common.Services
    open FsharpTodoApp.Domain.Common.ValueObjects
    open FsharpTodoApp.Domain.Features.Todo.Entities
    open FsharpTodoApp.Domain.Features.Todo.Policies

    type Dependencies = { DateTime: IDateTimeProvider }

    let buildCreated deps actor (title, description, dueDate, assignee, reviewer) =
        let ctx = TodoCreationPolicy actor |> AuditContext.create deps.DateTime
        TodoEntity.tryCreate ctx (title, description, dueDate, assignee, reviewer)

    let buildUpdated deps actor (title, description, dueDate, assignee, reviewer) entity =
        let ctx = TodoUpdatePolicy(actor, entity) |> AuditContext.create deps.DateTime

        entity
        |> TodoEntity.tryUpdate ctx (title, description, dueDate, assignee, reviewer)

    let buildStatusUpdated deps actor newStatus entity =
        let ctx =
            TodoUpdateStatusPolicy(actor, entity, newStatus)
            |> AuditContext.create deps.DateTime

        entity |> TodoEntity.tryUpdateStatus ctx newStatus

    let buildDeleted deps actor entity =
        let ctx = TodoDeletionPolicy(actor, entity) |> AuditContext.create deps.DateTime

        entity |> TodoEntity.tryDelete ctx
