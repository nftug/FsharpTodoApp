namespace FsharpTodoApp.Domain.Common.Entities

open FsharpTodoApp.Domain.Common.ValueObjects

type EntityBase =
    { IdSet: EntityIdSet
      CreatedAudit: EntityAudit
      UpdatedAudit: EntityAudit option
      DeletedAudit: EntityAudit option }

module EntityBase =
    open FsharpTodoApp.Domain.Common.Errors

    let create ctx =
        match ctx.Policy.CanCreate with
        | true ->
            Ok
                { IdSet = EntityIdSet.create ()
                  CreatedAudit = EntityAudit.create ctx
                  UpdatedAudit = None
                  DeletedAudit = None }
        | false -> Error ForbiddenError

    let update ctx this =
        match ctx.Policy.CanUpdate with
        | true ->
            Ok
                { this with
                    UpdatedAudit = Some(EntityAudit.create ctx) }
        | false -> Error ForbiddenError

    let delete ctx this =
        match ctx.Policy.CanDelete with
        | true ->
            Ok
                { this with
                    DeletedAudit = Some(EntityAudit.create ctx) }
        | false -> Error ForbiddenError
