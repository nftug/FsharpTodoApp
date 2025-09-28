namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Services

type EntityBase =
    { IdSet: EntityIdSet
      CreatedAudit: EntityAudit
      UpdatedAudit: EntityAudit option
      DeletedAudit: EntityAudit option }

module EntityBase =
    let create (actor: Actor) (dateTimeProvider: IDateTimeProvider) =
        { IdSet = EntityIdSet.create ()
          CreatedAudit = EntityAudit.create actor dateTimeProvider
          UpdatedAudit = None
          DeletedAudit = None }

    let update (actor: Actor) (dateTimeProvider: IDateTimeProvider) this =
        { this with
            UpdatedAudit = Some(EntityAudit.create actor dateTimeProvider) }

    let delete (actor: Actor) (dateTimeProvider: IDateTimeProvider) this =
        { this with
            DeletedAudit = Some(EntityAudit.create actor dateTimeProvider) }
