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

    let private loadTrackedAsync spec =
        async {
            if spec.EntityBase |> EntityBase.isNew then
                return None
            else
                let! existing =
                    spec.Query
                        .AsTracking()
                        .SingleOrDefaultAsync(fun x -> x.Id = spec.EntityBase.IdSet.DbId)
                    |> Async.AwaitTask

                return Option.ofObj existing
        }

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

    let saveAsync (ctx: AppDbContext) spec =
        async {
            use! tx = ctx.Database.BeginTransactionAsync() |> Async.AwaitTask

            let! current = loadTrackedAsync spec
            let instance = current |> ensureDehydrate ctx spec

            let! _ = ctx.SaveChangesAsync() |> Async.AwaitTask

            // Transfer intermediates after saving to ensure they can reference the saved entity
            match spec.AfterSave with
            | Some afterSave ->
                afterSave instance

                if ctx.ChangeTracker.HasChanges() then
                    let! _ = ctx.SaveChangesAsync() |> Async.AwaitTask
                    ()
            | None -> ()

            do! tx.CommitAsync() |> Async.AwaitTask

            ctx.ChangeTracker.Clear()

            return spec.EntityBase |> EntityBase.setDbId instance.Id
        }
