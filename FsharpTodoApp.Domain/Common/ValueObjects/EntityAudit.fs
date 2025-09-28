namespace FsharpTodoApp.Domain.Common.ValueObjects

open FsharpTodoApp.Domain.Common.Services

type EntityAudit =
    { UserInfo: UserInfo
      Timestamp: System.DateTime }

type CreatedAudit = private CreatedAudit of EntityAudit
type UpdatedAudit = private UpdatedAudit of EntityAudit option
type DeletedAudit = private DeletedAudit of EntityAudit option

module EntityAudit =
    let create (actor: Actor) (dateTimeProvider: IDateTimeProvider) =
        { UserInfo = actor.UserInfo
          Timestamp = dateTimeProvider.UtcNow }

module CreatedAudit =
    let create actor dateTimeProvider =
        EntityAudit.create actor dateTimeProvider |> CreatedAudit

    let value (CreatedAudit audit) = audit

module UpdatedAudit =
    let create actor dateTimeProvider =
        EntityAudit.create actor dateTimeProvider |> Some |> UpdatedAudit

    let none = UpdatedAudit None

    let value (UpdatedAudit audit) = audit

module DeletedAudit =
    let create actor dateTimeProvider =
        EntityAudit.create actor dateTimeProvider |> Some |> DeletedAudit

    let none = DeletedAudit None

    let value (DeletedAudit audit) = audit
