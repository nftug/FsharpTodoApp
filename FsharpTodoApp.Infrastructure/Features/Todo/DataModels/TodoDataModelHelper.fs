namespace FsharpTodoApp.Infrastructure.Features.Todo.DataModels

open FsharpTodoApp.Infrastructure.Persistence.DataModels
open FsharpTodoApp.Domain.Features.Todo.Entities
open FsharpTodoApp.Domain.Features.Todo.ValueObjects
open FsharpTodoApp.Persistence.DataModels
open System.Linq.Expressions
open System
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Infrastructure.Persistence.Repositories

module TodoDataModelHelper =
    let dehydrate (dataModel: TodoDataModel) (domain: TodoEntity) : unit =
        domain.Base |> DataModelBaseHelper.dehydrate dataModel

        dataModel.Title <- domain.Title |> TodoTitle.value
        dataModel.Description <- domain.Description |> TodoDescription.value |> Option.defaultValue null
        dataModel.DueDate <- domain.DueDate |> TodoDueDate.value |> Option.toNullable
        dataModel.Status <- domain.Status |> TodoStatus.value |> TodoStatus.toEnum
        dataModel.Assignee <- domain.Assignee |> TodoAssignee.value |> Option.map _.UserName |> Option.defaultValue null
        dataModel.Reviewer <- domain.Reviewer |> TodoReviewer.value |> Option.map _.UserName |> Option.defaultValue null

    let filterQueryExpression (actor: Actor option): Expression<Func<TodoDataModel, bool>> =
        ExprHelper.toExpression <@ fun (t: TodoDataModel) ->
            match actor with
            | Some a ->
                a.Roles |> ActorRole.isAtLeast Admin
                || t.CreatedBy = a.UserInfo.UserName
                || t.Assignee = a.UserInfo.UserName
                || t.Reviewer = a.UserInfo.UserName
            | None -> false @>
