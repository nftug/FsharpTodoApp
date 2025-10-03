namespace FsharpTodoApp.Infrastructure.Features.Todo.DataModels

open System
open FsharpTodoApp.Infrastructure.Persistence.DataModels
open FsharpTodoApp.Domain.Features.Todo.Enums
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open Microsoft.EntityFrameworkCore.Storage.ValueConversion

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
    let onModelCreating modelBuilder =
        DataModelBase.onModelCreating<TodoDataModel> "Todos" modelBuilder

        modelBuilder.Entity<TodoDataModel>()
            .Property(fun x -> x.Status)
            .HasConversion(new ValueConverter<TodoStatusEnum, string>(
                (fun v -> v.ToString()),
                (fun v -> Enum.Parse<TodoStatusEnum> v)
            )) |> ignore

    let dehydrate (dataModel: TodoDataModel) (domain: TodoEntity) =
        domain.Base |> DataModelBase.dehydrate dataModel

        dataModel.Title <- domain.Title |> TodoTitle.value
        dataModel.Description <- domain.Description |> TodoDescription.value
        dataModel.DueDate <- domain.DueDate |> TodoDueDate.value
        dataModel.Status <- domain.Status |> TodoStatus.value |> TodoStatusEnum.fromDomain
        dataModel.Assignee <- domain.Assignee |> TodoAssignee.value |> Option.map _.UserName
        dataModel.Reviewer <- domain.Reviewer |> TodoReviewer.value |> Option.map _.UserName
