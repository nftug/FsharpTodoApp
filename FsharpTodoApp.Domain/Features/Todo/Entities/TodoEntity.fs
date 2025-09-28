namespace FsharpTodoApp.Domain.Features.Todo.Entities

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.ValueObjects

type TodoEntity =
    { Base: EntityBase
      Title: TodoTitle
      Description: TodoDescription
      DueDate: TodoDueDate
      Status: TodoStatus }

module TodoEntity =
    open FsToolkit.ErrorHandling

    let create actorPolicy dateTimeProvider title description dueDate =
        result {
            let! validBase = EntityBase.create actorPolicy dateTimeProvider
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate

            return
                { Base = validBase
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = TodoStatus.start }
        }

    let update actorPolicy dateTimeProvider title description dueDate status this =
        result {
            let! validBase = this.Base |> EntityBase.update actorPolicy dateTimeProvider
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate
            let! validStatus = this.Status |> TodoStatus.tryUpdate status

            return
                { Base = validBase
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = validStatus }
        }

    let delete actorPolicy dateTimeProvider this =
        result {
            let! validBase = this.Base |> EntityBase.delete actorPolicy dateTimeProvider
            return { this with Base = validBase }
        }
