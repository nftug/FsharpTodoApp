namespace FsharpTodoApp.Domain.Features.Todo.Services

open FsharpTodoApp.Domain.Common.Services
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.Policies

type TodoPolicyService(dtProvider: IDateTimeProvider) =
    member _.BuildNewEntity actor title description dueDate assignee reviewer =
        let ctx = TodoCreationPolicy actor |> AuditContext.create dtProvider
        TodoEntity.create ctx title description dueDate assignee reviewer

    member _.BuildUpdatedEntity actor title description dueDate assignee reviewer entity =
        let ctx = TodoUpdatePolicy(actor, entity) |> AuditContext.create dtProvider
        entity |> TodoEntity.update ctx title description dueDate assignee reviewer

    member _.BuildUpdatedStatus actor newStatus entity =
        let ctx =
            TodoUpdateStatusPolicy(actor, entity, newStatus)
            |> AuditContext.create dtProvider

        entity |> TodoEntity.updateStatus ctx newStatus

    member _.BuildDeletedEntity actor entity =
        let ctx = TodoDeletionPolicy(actor, entity) |> AuditContext.create dtProvider
        entity |> TodoEntity.delete ctx
