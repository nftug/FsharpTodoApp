namespace FsharpTodoApp.Domain.Common.ValueObjects

type EntityBase =
    { IdSet: EntityIdSet
      CreatedAudit: CreatedAudit
      UpdatedAudit: UpdatedAudit
      DeletedAudit: DeletedAudit }

module EntityBase =
    open FsharpTodoApp.Domain.Features.Auth.Policies

    let create (actorPolicy: IActorPolicy) dateTimeProvider =
        match actorPolicy.CanCreate with
        | true ->
            Ok
                { IdSet = EntityIdSet.create ()
                  CreatedAudit = CreatedAudit.create actorPolicy.Actor dateTimeProvider
                  UpdatedAudit = UpdatedAudit.none
                  DeletedAudit = DeletedAudit.none }
        | false -> Error "Actor does not have permission to create entity."

    let update (actorPolicy: IActorPolicy) dateTimeProvider this =
        match actorPolicy.CanUpdate with
        | true ->
            Ok
                { this with
                    UpdatedAudit = UpdatedAudit.create actorPolicy.Actor dateTimeProvider }
        | false -> Error "Actor does not have permission to update entity."

    let delete (actorPolicy: IActorPolicy) dateTimeProvider this =
        match actorPolicy.CanDelete with
        | true ->
            Ok
                { this with
                    DeletedAudit = DeletedAudit.create actorPolicy.Actor dateTimeProvider }
        | false -> Error "Actor does not have permission to delete entity."
