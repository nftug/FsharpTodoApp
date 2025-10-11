namespace FsharpTodoApp.Domain.Compositions

module DomainServiceInjector =
    open Microsoft.Extensions.DependencyInjection
    open FsharpTodoApp.Domain.Common.Services
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsharpTodoApp.Domain.Features.User.Interfaces
    open FsharpTodoApp.Domain.Features.User.Services

    let inject (services: IServiceCollection) : IServiceCollection =
        services
            .AddSingleton<DateTimeProvider>(fun _ -> DateTimeProvider.create ())
            .AddScoped<UserPolicyService>(fun sp ->
                let datetime = sp.GetRequiredService<DateTimeProvider>()
                let userRef = sp.GetRequiredService<UserReferenceService>()
                UserPolicyService.create datetime userRef)
            .AddScoped<TodoPolicyService>(fun sp ->
                let datetime = sp.GetRequiredService<DateTimeProvider>()
                TodoPolicyService.create datetime)
        |> ignore

        services
