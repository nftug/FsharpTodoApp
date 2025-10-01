namespace FsharpTodoApp.Infrastructure.Persistence.Utils

module RepositoryHelper =
    open FsharpTodoApp.Domain.Common.Entities
    open FsharpTodoApp.Infrastructure.Persistence
    open FsharpTodoApp.Infrastructure.Persistence.DataModels
    open Microsoft.EntityFrameworkCore
    open System.Linq

    let saveAsync<'T when 'T :> DataModelBase and 'T: (new: unit -> 'T) and 'T: not struct>
        (ctx: AppDbContext)
        (queryableFn: AppDbContext -> IQueryable<'T>)
        (entityBase: EntityBase)
        (fromDomain: 'T -> unit)
        (fromDomainForIntermediates: ('T -> unit) option)
        =
        async {
            use! transaction = ctx.Database.BeginTransactionAsync() |> Async.AwaitTask

            let! existing =
                (queryableFn ctx)
                    .AsTracking()
                    .SingleOrDefaultAsync(fun x -> x.Id = entityBase.IdSet.DbId)
                |> Async.AwaitTask

            let updated =
                match existing with
                | null ->
                    let newDataModel = new 'T()
                    fromDomain newDataModel
                    ctx.Add newDataModel |> ignore
                    newDataModel
                | existing ->
                    fromDomain existing
                    existing

            let! _ = ctx.SaveChangesAsync() |> Async.AwaitTask

            // Transfer intermediates after saving to ensure they can reference the saved entity
            fromDomainForIntermediates |> Option.iter (fun f -> f updated)

            if ctx.ChangeTracker.HasChanges() then
                let! _ = ctx.SaveChangesAsync() |> Async.AwaitTask
                ()

            do! transaction.CommitAsync() |> Async.AwaitTask

            ctx.ChangeTracker.Clear()

            return entityBase |> EntityBase.setDbId updated.Id
        }
