namespace FsharpTodoApp.Presentation.Compositions

open Microsoft.Extensions.DependencyInjection
open FsharpTodoApp.Presentation.Services
open FsharpTodoApp.Application.Features.User.Commands

module PresentationComposition =
    let inject (services: IServiceCollection) : IServiceCollection =
        services.AddScoped<OidcActorFactoryService>(fun sp ->
            let getOrCreateUser = sp.GetRequiredService<GetOrCreateUser>()
            OidcActorFactoryService.create getOrCreateUser)
        |> ignore

        services
