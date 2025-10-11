namespace FsharpTodoApp.Domain.Common.Entities

open System
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Errors

type EntityBase =
    { IdSet: EntityIdSet
      CreatedAudit: CreatedAudit
      UpdatedAudit: UpdatedAudit
      DeletedAudit: DeletedAudit }

module EntityBase =
    let tryCreateWithPublicId (ctx: AuditContext) (publicId: Guid) : Result<EntityBase, AppError> =
        match ctx.Permission.CanCreate with
        | true ->
            Ok
                { IdSet = { DbId = 0L; PublicId = publicId }
                  CreatedAudit = CreatedAudit.create ctx
                  UpdatedAudit = UpdatedAudit.none
                  DeletedAudit = DeletedAudit.none }
        | false -> Error ForbiddenError

    let tryCreate (ctx: AuditContext) : Result<EntityBase, AppError> =
        tryCreateWithPublicId ctx (System.Guid.NewGuid())

    let tryUpdate (ctx: AuditContext) (this: EntityBase) : Result<EntityBase, AppError> =
        match ctx.Permission.CanUpdate with
        | true ->
            Ok
                { this with
                    UpdatedAudit = UpdatedAudit.create ctx }
        | false -> Error ForbiddenError

    let tryDelete (ctx: AuditContext) (this: EntityBase) : Result<EntityBase, AppError> =
        match ctx.Permission.CanDelete with
        | true ->
            Ok
                { this with
                    DeletedAudit = DeletedAudit.create ctx }
        | false -> Error ForbiddenError

    let isNew (this: EntityBase) : bool = this.IdSet.DbId = 0L

    let ofDbId (this: EntityBase) : Option<int64> =
        match this.IdSet.DbId with
        | dbId when dbId > 0L -> Some dbId
        | _ -> None

    let isDeleted (this: EntityBase) : bool = this.DeletedAudit <> DeletedAudit.none

    let setDbId (dbId: int64) (this: EntityBase) : EntityBase = { this with IdSet.DbId = dbId }

    let hydrate
        (
            dbId: int64,
            publicId: Guid,
            createdAudit: CreatedAudit,
            updatedAudit: UpdatedAudit,
            deletedAudit: DeletedAudit
        ) : EntityBase =
        { IdSet = { DbId = dbId; PublicId = publicId }
          CreatedAudit = createdAudit
          UpdatedAudit = updatedAudit
          DeletedAudit = deletedAudit }
