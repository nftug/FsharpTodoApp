namespace FsharpTodoApp.Domain.Common.Entities

open FsharpTodoApp.Domain.Common.ValueObjects

type EntityBase =
    { IdSet: EntityIdSet
      CreatedAudit: CreatedAudit
      UpdatedAudit: UpdatedAudit
      DeletedAudit: DeletedAudit }

module EntityBase =
    open FsharpTodoApp.Domain.Common.Errors

    let tryCreateWithPublicId ctx publicId =
        match ctx.Permission.CanCreate with
        | true ->
            Ok
                { IdSet = { DbId = 0L; PublicId = publicId }
                  CreatedAudit = CreatedAudit.create ctx
                  UpdatedAudit = UpdatedAudit.none
                  DeletedAudit = DeletedAudit.none }
        | false -> Error ForbiddenError

    let tryCreate ctx =
        tryCreateWithPublicId ctx (System.Guid.NewGuid())

    let tryUpdate ctx this =
        match ctx.Permission.CanUpdate with
        | true ->
            Ok
                { this with
                    UpdatedAudit = UpdatedAudit.create ctx }
        | false -> Error ForbiddenError

    let tryDelete ctx this =
        match ctx.Permission.CanDelete with
        | true ->
            Ok
                { this with
                    DeletedAudit = DeletedAudit.create ctx }
        | false -> Error ForbiddenError

    let isNew this = this.IdSet.DbId = 0L

    let ofDbId this =
        match this.IdSet.DbId with
        | dbId when dbId > 0L -> Some dbId
        | _ -> None

    let isDeleted this = this.DeletedAudit <> DeletedAudit.none

    let setDbId dbId this = { this with IdSet.DbId = dbId }

    let hydrate (dbId, publicId) (createdAt, createdBy) (updatedAt, updatedBy) (deletedAt, deletedBy) =
        { IdSet = { DbId = dbId; PublicId = publicId }
          CreatedAudit = CreatedAudit.hydrate (createdAt, createdBy)
          UpdatedAudit = UpdatedAudit.hydrate (updatedAt, updatedBy)
          DeletedAudit = DeletedAudit.hydrate (deletedAt, deletedBy) }
