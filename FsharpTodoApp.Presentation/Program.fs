namespace FsharpTodoApp.Presentation

#nowarn "20"

open System
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.IdentityModel.Tokens
open FsharpTodoApp.Domain.Compositions
open FsharpTodoApp.Application.Compositions
open FsharpTodoApp.Infrastructure.Compositions
open Giraffe
open FsharpTodoApp.Presentation.Endpoints

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        let configuration = builder.Configuration

        builder.Services.AddControllers()
        builder.Services.AddGiraffe() |> ignore

        DomainServiceInjector.inject builder.Services
        |> ApplicationServiceInjector.inject
        |> InfrastructureServiceInjector.inject configuration
        |> PresentationServiceInjector.inject

        let authSection = configuration.GetSection "Authentication"
        let authority = authSection.GetValue "Authority"
        let audience = authSection.GetValue "Audience"

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun options ->
                options.Authority <- authority
                options.RequireHttpsMetadata <- (new Uri(authority)).Scheme = Uri.UriSchemeHttps

                options.TokenValidationParameters <-
                    TokenValidationParameters(
                        ValidateIssuer = false,
                        // ValidIssuer = authority,
                        ValidateAudience = false,
                        // ValidAudience = audience,
                        NameClaimType = "preferred_username"
                    ))
        |> ignore

        builder.Services.AddAuthorization() |> ignore

        let app = builder.Build()

        using (app.Services.CreateScope()) (fun scope ->
            let services = scope.ServiceProvider

            let dbContext =
                services.GetRequiredService<FsharpTodoApp.Persistence.AppDbContext>()

            dbContext.Database.Migrate())

        app.UseHttpsRedirection()

        app.UseAuthentication()
        app.UseAuthorization()
        app.UseGiraffe(choose [ TodoHandlers.routes ])

        app.Run()

        exitCode
