namespace FsharpTodoApp.Presentation

#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open FsharpTodoApp.Domain.Compositions
open FsharpTodoApp.Application.Compositions
open FsharpTodoApp.Infrastructure.Compositions

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        let configuration = builder.Configuration

        builder.Services.AddControllers()

        DomainServiceInjector.inject builder.Services
        |> ApplicationServiceInjector.inject
        |> InfrastructureServiceInjector.inject configuration
        |> PresentationServiceInjector.inject

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.Run()

        exitCode
