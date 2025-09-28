namespace FsharpTodoApp.Domain.Features.Todo.Entities

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Domain.Common.Entities

type TodoEntity =
    { Base: EntityBase
      Title: TodoTitle
      Description: TodoDescription
      DueDate: TodoDueDate
      Status: TodoStatus }

module TodoEntity =
    open FsharpTodoApp.Domain.Features.Auth.Policies
    open FsToolkit.ErrorHandling

    let create (ctx: AuditContext<OwnerOnlyActorPolicy>) title description dueDate =
        result {
            let! validBase = EntityBase.create ctx
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate ctx

            return
                { Base = validBase
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = TodoStatus.start }
        }

    let update (ctx: AuditContext<OwnerOnlyActorPolicy>) title description dueDate status this =
        result {
            let! validBase = this.Base |> EntityBase.update ctx
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate ctx
            let! validStatus = this.Status |> TodoStatus.tryUpdate status

            return
                { Base = validBase
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = validStatus }
        }

    let delete (ctx: AuditContext<OwnerOnlyActorPolicy>) this =
        result {
            let! validBase = this.Base |> EntityBase.delete ctx
            return { this with Base = validBase }
        }
