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

    let save (ctx: AppDbContext) (spec: SaveSpec<'T>) : Task<EntityBase> =
        task {
            use! tx = ctx.Database.BeginTransactionAsync()

            let! instance =
                match spec.EntityBase |> EntityBase.ofDbId with
                | None -> task { return None }
                | Some dbId -> spec.Query.AsTracking().SingleAsync(fun x -> x.Id = dbId) |> Task.map Some
                |> Task.map (function
                    | Some existing ->
                        spec.Dehydrate existing
                        existing
                    | None ->
                        let newDataModel = new 'T()
                        spec.Dehydrate newDataModel
                        ctx.Add newDataModel |> ignore
                        newDataModel)

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
