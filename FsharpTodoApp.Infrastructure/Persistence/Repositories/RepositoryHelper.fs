namespace FsharpTodoApp.Infrastructure.Persistence.Repositories

open FsharpTodoApp.Domain.Common.Entities
open System.Linq
open FsharpTodoApp.Persistence.DataModels

type SaveSpec<'T when 'T :> DataModelBase and 'T: (new: unit -> 'T) and 'T: not struct> =
    { EntityBase: EntityBase
      Query: IQueryable<'T>
      Dehydrate: 'T -> unit
      AfterSave: ('T -> unit) option }

module RepositoryHelper =
    open Microsoft.EntityFrameworkCore
    open FsToolkit.ErrorHandling
    open FsharpTodoApp.Persistence
    open System.Threading.Tasks

    let private findOrCreate (ctx: AppDbContext) (spec: SaveSpec<'T>) : Task<'T> =
        task {
            match spec.EntityBase |> EntityBase.ofDbId with
            | None ->
                let newDataModel = new 'T()
                spec.Dehydrate newDataModel
                ctx.Add newDataModel |> ignore
                return newDataModel
            | Some id ->
                let! existing = spec.Query.AsTracking().SingleAsync(fun x -> x.Id = id)
                spec.Dehydrate existing
                return existing
        }

    let save (ctx: AppDbContext) (spec: SaveSpec<'T>) : Task<EntityBase> =
        task {
            use! tx = ctx.Database.BeginTransactionAsync()

            try
                let! instance = findOrCreate ctx spec
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
            with e ->
                do! tx.RollbackAsync()
                return! Task.FromException<EntityBase> e
        }
