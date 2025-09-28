namespace FsharpTodoApp.Domain.Features.Todo.Entities

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsToolkit.ErrorHandling

type TodoEntity =
    { Base: EntityBase
      Title: TodoTitle
      Description: TodoDescription
      DueDate: TodoDueDate
      Status: TodoStatus }

module TodoEntity =
    let create actor dateTimeProvider title description dueDate =
        result {
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate

            return
                { Base = EntityBase.create actor dateTimeProvider
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = TodoStatus.start }
        }

    let update actor dateTimeProvider title description dueDate status this =
        result {
            let! validTitle = title |> TodoTitle.tryCreate
            let! validDescription = description |> TodoDescription.tryCreate
            let! validDueDate = dueDate |> TodoDueDate.tryCreate
            let! validStatus = this.Status |> TodoStatus.tryUpdate status

            return
                { Base = this.Base |> EntityBase.update actor dateTimeProvider
                  Title = validTitle
                  Description = validDescription
                  DueDate = validDueDate
                  Status = validStatus }
        }

    let delete actor dateTimeProvider this =
        { this with
            Base = this.Base |> EntityBase.delete actor dateTimeProvider }
