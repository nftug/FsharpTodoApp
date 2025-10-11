namespace FsharpTodoApp.Infrastructure.Features.User.Repositories

open System.Linq
open Microsoft.EntityFrameworkCore
open FsharpTodoApp.Domain.Features.User.Entities
open FsToolkit.ErrorHandling
open FsharpTodoApp.Infrastructure.Persistence.Repositories
open FsharpTodoApp.Infrastructure.Features.User.DataModels
open FsharpTodoApp.Domain.Features.User.Interfaces
open FsharpTodoApp.Persistence
open FsharpTodoApp.Domain.Common.Entities
open FsharpTodoApp.Domain.Common.ValueObjects

module UserRepositoryImpl =
    let private getUserByUserName (ctx: AppDbContext) _ userName =
        ctx.Users
            .Where(fun x -> x.UserName = userName)
            .Select(fun x ->
                { Base =
                    EntityBase.hydrate
                        (x.Id, x.PublicId)
                        (x.CreatedAt, x.CreatedBy)
                        (Option.ofNullable x.UpdatedAt, Option.ofObj x.UpdatedBy)
                        (Option.ofNullable x.DeletedAt, Option.ofObj x.DeletedBy)
                  UserName = x.UserName
                  FullName = Option.ofObj x.FullName
                  Roles = ActorRole.fromEnums x.Roles })
            .SingleOrDefaultAsync()
        |> Task.map Option.ofObj

    let private saveUser (ctx: AppDbContext) _ entity =
        { EntityBase = entity.Base
          Query = ctx.Users
          Dehydrate = fun d -> entity |> UserDataModelHelper.dehydrate d
          AfterSave = None }
        |> RepositoryHelper.save ctx
        |> Task.map (fun newBase -> { entity with Base = newBase })

    let create (ctx: AppDbContext) : UserRepository =
        { GetUserByUserName = getUserByUserName ctx
          SaveUser = saveUser ctx }
