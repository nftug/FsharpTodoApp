namespace FsharpTodoApp.Presentation.Services

open System.Security.Claims
open System.Text.Json
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Application.Features.User.Commands
open FsharpTodoApp.Domain.Features.User.Entities

type OidcActorFactoryService =
    { Handle: HttpContext -> TaskResult<Actor option, AppError> }

module OidcActorFactoryService =
    type private KeycloakRealmAccess = { roles: string list }

    let private ofRoleString (role: string) =
        match role.ToLower() with
        | "user" -> User
        | "manager" -> Manager
        | "admin" -> Admin
        | _ -> User

    let private getOrCreateActor (useCase: GetOrCreateUser) (ctx: HttpContext) =
        taskResult {
            if not ctx.User.Identity.IsAuthenticated then
                return None
            else
                let userId =
                    ctx.User.FindFirstValue "sub"
                    |> Option.ofObj
                    |> Option.bind (fun s ->
                        match System.Guid.TryParse s with
                        | true, guid -> Some guid
                        | false, _ -> None)

                let username = ctx.User.FindFirstValue "preferred_username" |> Option.ofObj
                let fullname = ctx.User.FindFirstValue "name" |> Option.ofObj

                // When no roles are found, default to User role. No optional roles are supported.
                let roles =
                    ctx.User.FindFirstValue "realm_access"
                    |> Option.ofObj
                    |> Option.bind (fun s ->
                        try
                            let data = JsonSerializer.Deserialize<KeycloakRealmAccess>(s)
                            data.roles |> List.map ofRoleString |> Some
                        with _ ->
                            None)
                    |> Option.defaultValue []

                // When userId or username is missing, we cannot create an actor
                match userId, username with
                | Some id, Some name ->
                    let! user = useCase.Handle(name, id, fullname, roles)
                    return user |> UserEntity.toActor |> Some
                | _ -> return None
        }

    let create (useCase: GetOrCreateUser) = { Handle = getOrCreateActor useCase }
