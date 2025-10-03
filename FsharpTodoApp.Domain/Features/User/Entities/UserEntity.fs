namespace FsharpTodoApp.Domain.Features.User.Entities

open FsharpTodoApp.Domain.Common.Entities
open FsharpTodoApp.Domain.Common.ValueObjects

type UserEntity =
    { Base: EntityBase
      UserName: string
      FullName: string option
      Roles: ActorRole list }

module UserEntity =
    open FsToolkit.ErrorHandling

    let tryCreate ctx (username, userId, fullname, roles) =
        result {
            let! entityBase = EntityBase.tryCreateWithPublicId ctx userId

            return
                { Base = entityBase
                  UserName = username
                  FullName = fullname
                  Roles = roles }
        }

    let tryUpdate ctx (fullname, roles) this =
        result {
            let! entityBase = EntityBase.tryUpdate ctx this.Base

            return
                { this with
                    Base = entityBase
                    FullName = fullname
                    Roles = roles }
        }

    let tryDelete ctx this =
        result {
            let! entityBase = EntityBase.tryDelete ctx this.Base

            return { this with Base = entityBase }
        }

    let toActor this =
        { UserInfo = { UserName = this.UserName }
          UserDbId = this.Base.IdSet.DbId
          Roles = this.Roles }
