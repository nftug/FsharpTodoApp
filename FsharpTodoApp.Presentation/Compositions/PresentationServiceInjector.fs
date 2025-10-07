namespace FsharpTodoApp.Domain.Compositions

module PresentationServiceInjector =
    open Microsoft.Extensions.DependencyInjection
    open FsharpTodoApp.Presentation.Services
    open FsharpTodoApp.Application.Features.User.Commands

    let inject (services: IServiceCollection) : IServiceCollection =
        services.AddScoped<OidcActorFactoryService>(fun sp ->
            sp.GetRequiredService<GetOrCreateUser>() |> OidcActorFactoryService.create)
        |> ignore

        services
