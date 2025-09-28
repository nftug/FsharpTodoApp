namespace FsharpTodoApp.Domain.Features.Todo.Services

open FsharpTodoApp.Domain.Common.Services
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Auth.Policies
open FsharpTodoApp.Domain.Features.Todo.Entities

type TodoPolicyService(dateTimeProvider: IDateTimeProvider) =
    member private _.NewCtx actor entity =
        { Policy = OwnerOnlyActorPolicy(actor, entity)
          DateTimeProvider = dateTimeProvider }

    member this.BuildNewEntity actor title description dueDate =
        let ctx = this.NewCtx actor None
        TodoEntity.create ctx title description dueDate

    member this.BuildUpdatedEntity actor title description dueDate status entity =
        let ctx = this.NewCtx actor (Some entity.Base)
        entity |> TodoEntity.update ctx title description dueDate status

    member this.BuildDeletedEntity actor entity =
        let ctx = this.NewCtx actor (Some entity.Base)
        entity |> TodoEntity.delete ctx
