namespace FsharpTodoApp.Infrastructure.Features.Todo.DataModels

open FsharpTodoApp.Infrastructure.Persistence.DataModels
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Persistence.DataModels

module TodoDataModel =
    let dehydrate (dataModel: TodoDataModel) (domain: TodoEntity) =
        domain.Base |> DataModelBase.dehydrate dataModel

        dataModel.Title <- domain.Title |> TodoTitle.value
        dataModel.Description <- domain.Description |> TodoDescription.value |> Option.defaultValue null
        dataModel.DueDate <- domain.DueDate |> TodoDueDate.value |> Option.toNullable
        dataModel.Status <- domain.Status |> TodoStatus.value |> TodoStatus.toEnum
        dataModel.Assignee <- domain.Assignee |> TodoAssignee.value |> Option.map _.UserName |> Option.defaultValue null
        dataModel.Reviewer <- domain.Reviewer |> TodoReviewer.value |> Option.map _.UserName |> Option.defaultValue null
