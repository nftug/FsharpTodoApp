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
                sp.GetRequiredService<AppDbContext>() |> UserReferenceServiceImpl.create)
            .AddScoped<UserRepository>(fun sp -> sp.GetRequiredService<AppDbContext>() |> UserRepositoryImpl.create)
            .AddScoped<TodoRepository>(fun sp -> sp.GetRequiredService<AppDbContext>() |> TodoRepositoryImpl.create)
            .AddScoped<TodoQueryService>(fun sp -> sp.GetRequiredService<AppDbContext>() |> TodoQueryServiceImpl.create)
        |> ignore

        services
