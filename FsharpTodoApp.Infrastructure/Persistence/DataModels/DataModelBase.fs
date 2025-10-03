namespace FsharpTodoApp.Infrastructure.Persistence.DataModels

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Entities
open FsharpTodoApp.Persistence.DataModels

module DataModelBase =
    let dehydrateDeletionState (dataModel: DataModelBase) (deletedAudit: DeletedAudit) =
        DeletedAudit.value deletedAudit
        |> Option.iter (fun audit ->
            dataModel.DeletedAt <- Some audit.Timestamp |> Option.toNullable
            dataModel.DeletedBy <- Some audit.UserInfo.UserName |> Option.defaultValue null)

    let dehydrate (dataModel: DataModelBase) (entityBase: EntityBase) =
        dataModel.Id <- entityBase.IdSet.DbId
        dataModel.PublicId <- entityBase.IdSet.PublicId

        CreatedAudit.value entityBase.CreatedAudit
        |> fun audit ->
            dataModel.CreatedAt <- audit.Timestamp
            dataModel.CreatedBy <- audit.UserInfo.UserName

        UpdatedAudit.value entityBase.UpdatedAudit
        |> Option.iter (fun audit ->
            dataModel.UpdatedAt <- Some audit.Timestamp |> Option.toNullable
            dataModel.UpdatedBy <- Some audit.UserInfo.UserName |> Option.defaultValue null)

        entityBase.DeletedAudit |> dehydrateDeletionState dataModel
