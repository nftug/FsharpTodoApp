namespace FsharpTodoApp.Presentation

#nowarn "20"

open System
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.IdentityModel.Tokens
open FsharpTodoApp.Domain.Compositions
open FsharpTodoApp.Application.Compositions
open FsharpTodoApp.Infrastructure.Compositions
open Giraffe
open FsharpTodoApp.Presentation.Endpoints

module Program =
    open Microsoft.Extensions.Configuration
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

        let getRequired (section: IConfigurationSection) name =
            match section[name] with
            | null
            | "" -> failwithf "Configuration '%s' is required." name
            | value -> value

        let authority = getRequired authSection "Authority"
        let audience = getRequired authSection "Audience"

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun options ->
                options.Authority <- authority
                options.RequireHttpsMetadata <- (new Uri(authority)).Scheme = Uri.UriSchemeHttps

                options.TokenValidationParameters <-
                    TokenValidationParameters(
                        ValidateIssuer = true,
                        ValidIssuer = authority,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        NameClaimType = "preferred_username"
                    ))
        |> ignore

        builder.Services.AddAuthorization() |> ignore

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthentication()
        app.UseAuthorization()
        app.UseGiraffe(choose [ TodoHandlers.routes ])
        app.MapControllers()

        app.Run()

        exitCode
