namespace FsharpTodoApp.Domain.Features.User.Entities

open FsharpTodoApp.Domain.Common.Entities
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Common.Errors

type UserEntity =
    { Base: EntityBase
      UserName: string
      FullName: string option
      Roles: ActorRole list }

module UserEntity =
    open FsToolkit.ErrorHandling

    let tryCreate
        (ctx: AuditContext)
        (username: string, userId: System.Guid, fullname: string option, roles: ActorRole list)
        : Result<UserEntity, AppError> =
        result {
            let! entityBase = EntityBase.tryCreateWithPublicId ctx userId

            return
                { Base = entityBase
                  UserName = username
                  FullName = fullname
                  Roles = roles }
        }

    let tryUpdate
        (ctx: AuditContext)
        (fullname: string option, roles: ActorRole list)
        (this: UserEntity)
        : Result<UserEntity, AppError> =
        result {
            let! entityBase = EntityBase.tryUpdate ctx this.Base

            return
                { this with
                    Base = entityBase
                    FullName = fullname
                    Roles = roles }
        }

    let tryDelete (ctx: AuditContext) (this: UserEntity) : Result<UserEntity, AppError> =
        result {
            let! entityBase = EntityBase.tryDelete ctx this.Base

            return { this with Base = entityBase }
        }

    let toActor (this: UserEntity) : Actor =
        { UserInfo = { UserName = this.UserName }
          UserDbId = this.Base.IdSet.DbId
          Roles = this.Roles }
