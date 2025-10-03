namespace FsharpTodoApp.Infrastructure.Features.User.Services

module UserReferenceServiceImpl =
    open System.Linq
    open Microsoft.EntityFrameworkCore
    open FsToolkit.ErrorHandling
    open FsharpTodoApp.Domain.Common.ValueObjects
    open FsharpTodoApp.Domain.Features.User.Interfaces
    open FsharpTodoApp.Persistence

    let private getUserRefByUserName (ctx: AppDbContext) userName =
        ctx.Users
            .Where(fun x -> x.UserName = userName)
            .Select(fun e -> { UserName = e.UserName })
            .SingleOrDefaultAsync()
        |> Task.map Option.ofObj

    let create (ctx: AppDbContext) : UserReferenceService =
        { GetUserRefByUserName = getUserRefByUserName ctx }
