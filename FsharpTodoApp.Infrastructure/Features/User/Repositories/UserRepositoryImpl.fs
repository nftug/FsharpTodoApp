namespace FsharpTodoApp.Infrastructure.Features.User.Repositories

module UserRepositoryImpl =
    open System.Linq
    open Microsoft.EntityFrameworkCore
    open FsharpTodoApp.Domain.Features.User.Entities
    open FsharpTodoApp.Domain.Common.Entities
    open FsToolkit.ErrorHandling
    open FsharpTodoApp.Infrastructure.Persistence.Repositories
    open FsharpTodoApp.Infrastructure.Features.User.DataModels
    open FsharpTodoApp.Domain.Features.User.Interfaces
    open FsharpTodoApp.Persistence
    open FsharpTodoApp.Domain.Common.ValueObjects

    let private getUserByUserName (ctx: AppDbContext) _ userName =
        ctx.Users
            .Where(fun x -> x.UserName = userName)
            .Select(fun e ->
                { Base =
                    EntityBase.hydrate
                        (e.Id, e.PublicId)
                        (e.CreatedAt, e.CreatedBy)
                        (e.UpdatedAt |> Option.ofNullable, e.UpdatedBy |> Option.ofObj)
                        (e.DeletedAt |> Option.ofNullable, e.DeletedBy |> Option.ofObj)
                  UserName = e.UserName
                  FullName = e.FullName |> Option.ofObj
                  Roles = e.Roles |> Array.toList |> List.map ActorRole.fromEnum })
            .SingleOrDefaultAsync()
        |> Task.map Option.ofObj

    let private saveUser (ctx: AppDbContext) _ entity =
        { EntityBase = entity.Base
          Query = ctx.Users
          Dehydrate = fun d -> entity |> UserDataModel.dehydrate d
          AfterSave = None }
        |> RepositoryHelper.save ctx
        |> Task.map (fun newBase -> { entity with Base = newBase })

    let create (ctx: AppDbContext) : UserRepository =
        { GetUserByUserName = getUserByUserName ctx
          SaveUser = saveUser ctx }
