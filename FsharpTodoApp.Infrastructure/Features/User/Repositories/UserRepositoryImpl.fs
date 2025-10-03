namespace FsharpTodoApp.Infrastructure.Features.User.Repositories

module UserRepositoryImpl =
    open System.Linq
    open Microsoft.EntityFrameworkCore
    open FsharpTodoApp.Domain.Features.User.Entities
    open FsharpTodoApp.Infrastructure.Persistence
    open FsharpTodoApp.Domain.Common.Entities
    open FsharpTodoApp.Domain.Common.Enums
    open FsToolkit.ErrorHandling
    open FsharpTodoApp.Infrastructure.Persistence.Repositories
    open FsharpTodoApp.Infrastructure.Features.User.DataModels
    open FsharpTodoApp.Domain.Features.User.Interfaces

    let private getUserByUserName (ctx: AppDbContext) _ userName =
        ctx.Users
            .Where(fun x -> x.UserName = userName)
            .Select(fun e ->
                { Base =
                    EntityBase.hydrate
                        (e.Id, e.PublicId)
                        (e.CreatedAt, e.CreatedBy)
                        (e.UpdatedAt, e.UpdatedBy)
                        (e.DeletedAt, e.DeletedBy)
                  UserName = e.UserName
                  FullName = e.FullName
                  Roles = e.Roles |> List.map ActorRoleEnum.toDomain })
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
