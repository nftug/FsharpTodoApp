namespace FsharpTodoApp.Infrastructure.Persistence.Utils

open FsharpTodoApp.Infrastructure.Persistence.DataModels
open FsharpTodoApp.Domain.Common.Entities
open System.Linq

type SaveSpec<'T when 'T :> DataModelBase and 'T: (new: unit -> 'T) and 'T: not struct> =
    { EntityBase: EntityBase
      Query: IQueryable<'T>
      Dehydrate: 'T -> unit
      AfterSave: ('T -> unit) option }

module RepositoryHelper =
    open FsharpTodoApp.Infrastructure.Persistence
    open Microsoft.EntityFrameworkCore
    open FsToolkit.ErrorHandling

    let private loadTracked spec =
        match spec.EntityBase.IdSet.DbId with
        | dbId when dbId = 0L -> task { return None }
        | dbId -> spec.Query.SingleOrDefaultAsync(fun x -> x.Id = dbId) |> Task.map Option.ofObj

    let private ensureDehydrate (ctx: DbContext) spec existing =
        match existing with
        | Some existing ->
            spec.Dehydrate existing
            existing
        | None ->
            let newDataModel = new 'T()
            spec.Dehydrate newDataModel
            ctx.Add newDataModel |> ignore
            newDataModel

    let save (ctx: AppDbContext) spec =
        task {
            use! tx = ctx.Database.BeginTransactionAsync()

            let! tracked = loadTracked spec
            let instance = tracked |> ensureDehydrate ctx spec

            do! ctx.SaveChangesAsync() |> Task.ignore

            // Transfer intermediates after saving to ensure they can reference the saved entity
            match spec.AfterSave with
            | Some afterSave ->
                afterSave instance

                if ctx.ChangeTracker.HasChanges() then
                    do! ctx.SaveChangesAsync() |> Task.ignore
            | None -> ()

            do! tx.CommitAsync()

            ctx.ChangeTracker.Clear()

            return spec.EntityBase |> EntityBase.setDbId instance.Id
        }
