namespace FsharpTodoApp.Domain.Features.User.Services

open FsharpTodoApp.Domain.Common.ValueObjects
open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Features.User.Entities
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Features.User.Interfaces
open FsharpTodoApp.Domain.Features.User.Policies
open FsharpTodoApp.Domain.Common.Services
open System

type UserCreateArgs = string * Guid * string option * ActorRole list
type UserUpdateArgs = string option * ActorRole list

type UserPolicyService =
    { BuildCreated: Actor -> UserCreateArgs -> TaskResult<UserEntity, AppError>
      BuildUpdated: Actor -> UserUpdateArgs -> UserEntity -> Result<UserEntity, AppError>
      BuildDeleted: Actor -> UserEntity -> Result<UserEntity, AppError> }

module UserPolicyService =
    let private buildCreated (datetime, userRef) actor (username, userId, fullname, roles) =
        taskResult {
            let ctx = UserCreationPermission.create actor |> AuditContext.create datetime actor

            do!
                userRef.GetUserRefByUserName username
                |> TaskResult.requireNone (ValidationError "Username already exists")
                |> TaskResult.ignore

            return!
                UserEntity.tryCreate ctx (username, userId, fullname, roles)
                |> TaskResult.ofResult
        }

    let private buildUpdated datetime actor (fullname, roles) this =
        let ctx =
            UserUpdatePermission.create actor this |> AuditContext.create datetime actor

        this |> UserEntity.tryUpdate ctx (fullname, roles)

    let private buildDeleted datetime actor this =
        let ctx =
            UserDeletionPermission.create actor this |> AuditContext.create datetime actor

        this |> UserEntity.tryDelete ctx

    let create (datetime: DateTimeProvider) (userRef: UserReferenceService) : UserPolicyService =
        { BuildCreated = buildCreated (datetime, userRef)
          BuildUpdated = buildUpdated datetime
          BuildDeleted = buildDeleted datetime }
