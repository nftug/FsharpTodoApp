namespace FsharpTodoApp.Application.Features.User.Commands

open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Features.User.Entities
open FsharpTodoApp.Domain.Features.User.Services
open FsharpTodoApp.Domain.Features.User.Interfaces
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Common.ValueObjects
open System

type GetOrCreateUser =
    { Handle: (string * Guid * string option * ActorRole list) -> TaskResult<UserEntity, AppError> }

module GetOrCreateUser =
    let private handle (repo, policyService) (username, userId, fullname, roles) =
        taskResult {
            let actor = Actor.systemActor
            let! existing = repo.GetUserByUserName (Some actor) username

            match existing with
            | None ->
                let! created = policyService.BuildCreated actor (username, userId, fullname, roles)
                return! repo.SaveUser actor created
            | Some user ->
                if user.FullName <> fullname || user.Roles <> roles then
                    let! updated = policyService.BuildUpdated actor (fullname, roles) user
                    return! repo.SaveUser actor updated
                else
                    return user
        }

    let create repo policyService =
        { Handle = handle (repo, policyService) }
