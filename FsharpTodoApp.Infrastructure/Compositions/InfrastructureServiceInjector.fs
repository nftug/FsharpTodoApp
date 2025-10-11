namespace FsharpTodoApp.Infrastructure.Compositions

module InfrastructureServiceInjector =
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.EntityFrameworkCore
    open Microsoft.Extensions.Configuration
    open FsharpTodoApp.Domain.Features.User.Interfaces
    open FsharpTodoApp.Infrastructure.Features.User.Services
    open FsharpTodoApp.Infrastructure.Features.User.Repositories
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Infrastructure.Features.Todo.Repositories
    open FsharpTodoApp.Persistence
    open FsharpTodoApp.Application.Features.Todo.Interfaces
    open FsharpTodoApp.Infrastructure.Features.Todo.QueryServices

    let inject (configuration: IConfiguration) (services: IServiceCollection) : IServiceCollection =
        services
            .AddDbContext<AppDbContext>(fun options ->
                options.UseSqlite(
                    configuration.GetConnectionString "DefaultConnection",
                    fun opt -> opt.UseQuerySplittingBehavior QuerySplittingBehavior.SplitQuery |> ignore
                )
                |> ignore)
            .AddScoped<UserReferenceService>(fun sp ->
                let dbContext = sp.GetRequiredService<AppDbContext>()
                UserReferenceServiceImpl.create dbContext)
            .AddScoped<UserRepository>(fun sp ->
                let dbContext = sp.GetRequiredService<AppDbContext>()
                UserRepositoryImpl.create dbContext)
            .AddScoped<TodoRepository>(fun sp ->
                let dbContext = sp.GetRequiredService<AppDbContext>()
                TodoRepositoryImpl.create dbContext)
            .AddScoped<TodoQueryService>(fun sp ->
                let dbContext = sp.GetRequiredService<AppDbContext>()
                TodoQueryServiceImpl.create dbContext)
        |> ignore

        services
