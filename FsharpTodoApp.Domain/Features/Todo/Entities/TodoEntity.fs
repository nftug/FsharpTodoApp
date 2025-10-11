namespace FsharpTodoApp.Domain.Features.Todo.Entities

open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Common.Errors
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Domain.Common.Entities
open FsharpTodoApp.Domain.Common.ValueObjects

type TodoEntity =
    { Base: EntityBase
      Title: TodoTitle
      Description: TodoDescription
      DueDate: TodoDueDate
      Status: TodoStatus
      Assignee: TodoAssignee
      Reviewer: TodoReviewer }

module TodoEntity =
    let hydrate
        (
            entityBase: EntityBase,
            title: TodoTitle,
            description: TodoDescription,
            dueDate: TodoDueDate,
            status: TodoStatus,
            assignee: TodoAssignee,
            reviewer: TodoReviewer
        ) : TodoEntity =
        { Base = entityBase
          Title = title
          Description = description
          DueDate = dueDate
          Status = status
          Assignee = assignee
          Reviewer = reviewer }

    let tryCreate
        (ctx: AuditContext)
        (title: string,
         description: string option,
         dueDate: System.DateTime option,
         assignee: UserInfo option,
         reviewer: UserInfo option)
        : Result<TodoEntity, AppError> =
        result {
            let! validBase = EntityBase.tryCreate ctx
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate ctx
            let! validAssignee = assignee |> TodoAssignee.tryAssign ctx
            let! validReviewer = reviewer |> TodoReviewer.tryAssign ctx

            return
                { Base = validBase
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = TodoStatus.defaultStatus
                  Assignee = validAssignee
                  Reviewer = validReviewer }
        }

    let tryUpdate
        (ctx: AuditContext)
        (title: string,
         description: string option,
         dueDate: System.DateTime option,
         assignee: UserInfo option,
         reviewer: UserInfo option)
        (this: TodoEntity)
        : Result<TodoEntity, AppError> =
        result {
            let! validBase = this.Base |> EntityBase.tryUpdate ctx
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate ctx
            let! validAssignee = assignee |> TodoAssignee.tryReassign ctx this.Assignee
            let! validReviewer = reviewer |> TodoReviewer.tryReassign ctx this.Reviewer

            return
                { this with
                    Base = validBase
                    Title = validTitle
                    Description = validDescription
                    DueDate = validDueDate
                    Assignee = validAssignee
                    Reviewer = validReviewer }
        }

    let tryUpdateStatus
        (ctx: AuditContext)
        (newStatus: TodoStatusValue)
        (this: TodoEntity)
        : Result<TodoEntity, AppError> =
        result {
            let! validStatus = TodoStatus.tryUpdate ctx newStatus
            let! validBase = this.Base |> EntityBase.tryUpdate ctx

            return
                { this with
                    Base = validBase
                    Status = validStatus }
        }

    let tryDelete (ctx: AuditContext) (this: TodoEntity) : Result<TodoEntity, AppError> =
        result {
            let! validBase = this.Base |> EntityBase.tryDelete ctx
            return { this with Base = validBase }
        }
