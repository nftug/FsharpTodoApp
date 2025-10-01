namespace FsharpTodoApp.Domain.Common.Entities

open FsharpTodoApp.Domain.Common.ValueObjects

type EntityBase =
    { IdSet: EntityIdSet
      CreatedAudit: CreatedAudit
      UpdatedAudit: UpdatedAudit
      DeletedAudit: DeletedAudit }

module EntityBase =
    open FsharpTodoApp.Domain.Common.Errors

    let create ctx =
        match ctx.Policy.CanCreate with
        | true ->
            Ok
                { IdSet = EntityIdSet.create ()
                  CreatedAudit = CreatedAudit.create ctx
                  UpdatedAudit = UpdatedAudit.none
                  DeletedAudit = DeletedAudit.none }
        | false -> Error ForbiddenError

    let update ctx this =
        match ctx.Policy.CanUpdate with
        | true ->
            Ok
                { this with
                    UpdatedAudit = UpdatedAudit.create ctx }
        | false -> Error ForbiddenError

    let delete ctx this =
        match ctx.Policy.CanDelete with
        | true ->
            Ok
                { this with
                    DeletedAudit = DeletedAudit.create ctx }
        | false -> Error ForbiddenError

    let isNew this = this.IdSet.DbId = 0L

    let isDeleted this = this.DeletedAudit <> DeletedAudit.none

    let setDbId dbId this =
        { this with
            IdSet = { this.IdSet with DbId = dbId } }

    let recreate (dbId, publicId) (createdAt, createdBy) (updatedAt, updatedBy) (deletedAt, deletedBy) =
        { IdSet = { DbId = dbId; PublicId = publicId }
          CreatedAudit = CreatedAudit.recreate (createdAt, createdBy)
          UpdatedAudit = UpdatedAudit.recreate (updatedAt, updatedBy)
          DeletedAudit = DeletedAudit.recreate (deletedAt, deletedBy) }
