namespace FsharpTodoApp.Infrastructure.Features.Todo.DataModels

open System
open FsharpTodoApp.Infrastructure.Persistence.DataModels
open FsharpTodoApp.Application.Features.Todo.Enums

[<Sealed>]
type TodoDataModel() =
    inherit DataModelBase()

    member val Title = String.Empty with get, set
    member val Description: string option = None with get, set
    member val DueDate: DateTime option = None with get, set
    member val Status: TodoStatusEnum = TodoStatusEnum.Todo with get, set
    member val Assignee: string option = None with get, set
    member val Reviewer: string option = None with get, set

module TodoDataModel =
    open FsharpTodoApp.Domain.Features.Todo.Entities
    open FsharpTodoApp.Domain.Features.Todo.ValueObjects

    let onModelCreating modelBuilder =
        DataModelBase.onModelCreating<TodoDataModel> "Todos" modelBuilder

    let dehydrate (dataModel: TodoDataModel) (domain: TodoEntity) =
        domain.Base |> DataModelBase.dehydrate dataModel

        dataModel.Title <- domain.Title |> TodoTitle.value
        dataModel.Description <- domain.Description |> TodoDescription.value
        dataModel.DueDate <- domain.DueDate |> TodoDueDate.value
        dataModel.Status <- domain.Status |> TodoStatus.value |> TodoStatusEnum.fromDomain
        dataModel.Assignee <- domain.Assignee |> TodoAssignee.value |> Option.map (fun x -> x.UserName)
        dataModel.Reviewer <- domain.Reviewer |> TodoReviewer.value |> Option.map (fun x -> x.UserName)
