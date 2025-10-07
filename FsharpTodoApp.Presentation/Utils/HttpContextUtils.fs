namespace FsharpTodoApp.Presentation.Utils

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open System.Threading.Tasks
open Giraffe
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Presentation.Services
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Application.Common.Dtos

module HttpContextUtils =
    let created payload : HttpHandler = setStatusCode 201 >=> json payload

    let noContent: HttpHandler = Successful.NO_CONTENT

    let getService<'T> (ctx: HttpContext) : 'T =
        ctx.RequestServices.GetRequiredService<'T>()

    let requireActor (ctx: HttpContext) : Task<Result<Actor, AppError>> =
        task {
            let factory = getService<OidcActorFactoryService> ctx
            let! actorResult = factory.Handle ctx

            match actorResult with
            | Ok(Some actor) -> return Ok actor
            | Ok None -> return Error UnauthorizedError
            | Error error -> return Error error
        }

    let respondAppError (error: AppError) : HttpHandler =
        match error with
        | ValidationError message -> setStatusCode 400 >=> json {| error = message |}
        | NotFoundError -> setStatusCode 404 >=> json {| error = "Todo not found." |}
        | ForbiddenError ->
            setStatusCode 403
            >=> json {| error = "You do not have permission to perform this action." |}
        | UnauthorizedError -> setStatusCode 401 >=> json {| error = "Authentication is required." |}

    let handleResult (result: Result<'T, AppError>) (successHandler: 'T -> HttpHandler) : HttpHandler =
        fun next ctx ->
            task {
                match result with
                | Ok value -> return! successHandler value next ctx
                | Error error -> return! respondAppError error next ctx
            }

    let handleCommandResult
        (result: Result<unit, AppError>)
        (next: HttpFunc)
        (ctx: HttpContext)
        : Task<HttpContext option> =
        handleResult result (fun () -> noContent) next ctx

    let handleCreateResult
        (result: Result<ItemCreatedResponseDto, AppError>)
        (next: HttpFunc)
        (ctx: HttpContext)
        : Task<HttpContext option> =
        handleResult result created next ctx
