namespace FsharpTodoApp.Domain.Features.Todo.Services

open FsharpTodoApp.Domain.Common.Services
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.Policies
open FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoCreateArgs = string * string option * System.DateTime option * UserInfo option * UserInfo option
type TodoUpdateArgs = string * string option * System.DateTime option * UserInfo option * UserInfo option

type TodoPolicyService =
    { BuildCreated: Actor -> TodoCreateArgs -> Result<TodoEntity, AppError>
      BuildUpdated: Actor -> TodoUpdateArgs -> TodoEntity -> Result<TodoEntity, AppError>
      BuildStatusUpdated: Actor -> TodoStatusValue -> TodoEntity -> Result<TodoEntity, AppError>
      BuildDeleted: Actor -> TodoEntity -> Result<TodoEntity, AppError> }

module TodoPolicyService =
    let private buildCreated dateTime actor (title, description, dueDate, assignee, reviewer) =
        let ctx = TodoCreationPermission.create () |> AuditContext.create dateTime actor

        TodoEntity.tryCreate ctx (title, description, dueDate, assignee, reviewer)

    let private buildUpdated dateTime actor (title, description, dueDate, assignee, reviewer) entity =
        let ctx =
            TodoUpdatePermission.create actor entity |> AuditContext.create dateTime actor

        entity
        |> TodoEntity.tryUpdate ctx (title, description, dueDate, assignee, reviewer)

    let private buildStatusUpdated dateTime actor newStatus entity =
        let ctx =
            TodoUpdateStatusPermission.create actor entity newStatus
            |> AuditContext.create dateTime actor

        entity |> TodoEntity.tryUpdateStatus ctx newStatus

    let private buildDeleted dateTime actor entity =
        let ctx =
            TodoDeletionPermission.create actor entity |> AuditContext.create dateTime actor

        entity |> TodoEntity.tryDelete ctx

    let create (dateTimeSvc: DateTimeProvider) =
        { BuildCreated = buildCreated dateTimeSvc
          BuildUpdated = buildUpdated dateTimeSvc
          BuildStatusUpdated = buildStatusUpdated dateTimeSvc
          BuildDeleted = buildDeleted dateTimeSvc }
