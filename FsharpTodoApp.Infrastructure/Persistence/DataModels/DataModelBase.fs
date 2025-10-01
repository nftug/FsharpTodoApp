namespace FsharpTodoApp.Infrastructure.Persistence.DataModels

open System
open System.ComponentModel.DataAnnotations
open Microsoft.EntityFrameworkCore

[<AbstractClass>]
type DataModelBase() =
    [<Key>]
    member val Id = 0L with get, set

    member val PublicId = Guid.Empty with get, set

    member val CreatedAt = DateTime.MinValue with get, set
    member val UpdatedAt: DateTime option = None with get, set
    member val DeletedAt: DateTime option = None with get, set

    member val CreatedBy = String.Empty with get, set
    member val UpdatedBy: string option = None with get, set
    member val DeletedBy: string option = None with get, set

module DataModelBase =
    open FsharpTodoApp.Domain.Common.ValueObjects
    open FsharpTodoApp.Domain.Common.Entities

    let onModelCreating<'T when 'T :> DataModelBase and 'T: not struct>
        (tableName: string)
        (modelBuilder: ModelBuilder)
        =
        modelBuilder.Entity<'T>().HasIndex(fun e -> e.PublicId :> Object).IsUnique()
        |> ignore

        modelBuilder.Entity<'T>().HasQueryFilter _.DeletedAt.IsNone |> ignore

        modelBuilder.Entity<'T>().ToTable tableName |> ignore

    let dehydrateDeletionState (dataModel: DataModelBase) (deletedAudit: DeletedAudit) =
        DeletedAudit.value deletedAudit
        |> Option.iter (fun audit ->
            dataModel.DeletedAt <- Some audit.Timestamp
            dataModel.DeletedBy <- Some audit.UserInfo.UserName)

    let dehydrate (dataModel: DataModelBase) (entityBase: EntityBase) =
        dataModel.Id <- entityBase.IdSet.DbId
        dataModel.PublicId <- entityBase.IdSet.PublicId

        CreatedAudit.value entityBase.CreatedAudit
        |> fun audit ->
            dataModel.CreatedAt <- audit.Timestamp
            dataModel.CreatedBy <- audit.UserInfo.UserName

        UpdatedAudit.value entityBase.UpdatedAudit
        |> Option.iter (fun audit ->
            dataModel.UpdatedAt <- Some audit.Timestamp
            dataModel.UpdatedBy <- Some audit.UserInfo.UserName)

        entityBase.DeletedAudit |> dehydrateDeletionState dataModel
