namespace FsharpTodoApp.Domain.Features.Todo.Entities

open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Domain.Common.Entities

type TodoEntity =
    { Base: EntityBase
      Title: TodoTitle
      Description: TodoDescription
      DueDate: TodoDueDate
      Status: TodoStatus
      Reviewer: TodoReviewer }

module TodoEntity =
    open FsToolkit.ErrorHandling

    let create ctx title description dueDate reviewer =
        result {
            let! validBase = EntityBase.create ctx
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate ctx
            let! validReviewer = reviewer |> TodoReviewer.tryAssign ctx

            return
                { Base = validBase
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = TodoStatus.start
                  Reviewer = validReviewer }
        }

    let update ctx title description dueDate reviewer this =
        result {
            let! validBase = this.Base |> EntityBase.update ctx
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate ctx
            let! validReviewer = reviewer |> TodoReviewer.tryAssign ctx

            return
                { this with
                    Base = validBase
                    Title = validTitle
                    Description = validDescription
                    DueDate = validDueDate
                    Reviewer = validReviewer }
        }

    let updateStatus ctx newStatus this =
        result {
            let! validBase = this.Base |> EntityBase.update ctx
            let! validStatus = this.Status |> TodoStatus.tryUpdate ctx this.Reviewer newStatus

            return
                { this with
                    Base = validBase
                    Status = validStatus }
        }

    let delete ctx this =
        result {
            let! validBase = this.Base |> EntityBase.delete ctx
            return { this with Base = validBase }
        }
