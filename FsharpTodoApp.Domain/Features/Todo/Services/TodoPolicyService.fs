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
    let private buildCreated datetime actor (title, description, dueDate, assignee, reviewer) =
        let ctx = TodoCreationPermission.create () |> AuditContext.create datetime actor

        TodoEntity.tryCreate ctx (title, description, dueDate, assignee, reviewer)

    let private buildUpdated datetime actor (title, description, dueDate, assignee, reviewer) entity =
        let ctx =
            TodoUpdatePermission.create actor entity |> AuditContext.create datetime actor

        entity
        |> TodoEntity.tryUpdate ctx (title, description, dueDate, assignee, reviewer)

    let private buildStatusUpdated datetime actor newStatus entity =
        let ctx =
            TodoUpdateStatusPermission.create actor entity newStatus
            |> AuditContext.create datetime actor

        entity |> TodoEntity.tryUpdateStatus ctx newStatus

    let private buildDeleted datetime actor entity =
        let ctx =
            TodoDeletionPermission.create actor entity |> AuditContext.create datetime actor

        entity |> TodoEntity.tryDelete ctx

    let create (datetime: DateTimeProvider) =
        { BuildCreated = buildCreated datetime
          BuildUpdated = buildUpdated datetime
          BuildStatusUpdated = buildStatusUpdated datetime
          BuildDeleted = buildDeleted datetime }
